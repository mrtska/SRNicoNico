/**
 * コメントレンダラ
 * デフォルトウィンドウサイズは1366 x 768
 * 
 * コメントの仕様 (mrtska調べ)
 * コメントサイズごとにウィンドウに入る文字数が異なる
 * 
 * big: 1画面に7.8コメント
 * medium: 11.3コメント
 * small: 16.6コメント
 * 
 * 改行入りの場合
 * big: 3行以上の場合16コメント
 * medium: 5行以上の場合25.4コメント
 * small: 7行以上の場合38コメント
 * 
 * 改行入りで上記の行数未満またはenderコマンド使用時
 * big: 8.4コメント
 * medium: 13.1コメント
 * small: 21コメント
 */

const DEFAULT_WIDTH = 1366;
const DEFAULT_WIDTH_43 = 1024;
const DEFAULT_WIDTH_169 = 1366;
const DEFAULT_HEIGHT = 768;

// font-size: 54を1とした場合
const COMMENT_SIZE_NORMAL = 1;
const COMMENT_SIZE_SMALL = 0.69;
const COMMENT_SIZE_BIG = 1.425;

// ニコニコで使われるフォント
const defont: string = 'Arial, "ＭＳ Ｐゴシック", "MS PGothic", MSPGothic, MS-PGothic';
const gothic: string = '"游ゴシック", SimHei, Arial, "ＭＳ Ｐゴシック", sans-serif';
const mincho: string = '"游明朝", SimSun, Arial, "ＭＳ Ｐゴシック", "游ゴシック", serif';

export class CommentHandler {

    wrapper: HTMLDivElement;
    canvas: HTMLCanvasElement;
    renderContext: CanvasRenderingContext2D;

    visibility: CommentVisibility;

    layers: CommentLayer[] | undefined;

    previousRenderedTime: number = -1;
    /**
     * 流れるコメントの最終位置
     */
    endLine: number = 0;
    centerLine: number = 0;
    /**
     * 流れるコメントの開始位置
     */
    firstLine: number = 0;
    /**
     * 画面を4:3にした時の実際の横幅
     */
    width43: number = 0;
    /**
     * 画面を16:9にした時の実際の横幅
     */
    width169: number = 0;

    /**
     * デフォルトウィンドウサイズにこの値を掛けると実際のウィンドウサイズ相当になる係数
     * 実際のウィンドウサイズがデフォルトウィンドウサイズと同じだったらこの値は1
     */
    scalingFactor: number = 0;

    constructor() {

        this.wrapper = document.getElementById('comment') as HTMLDivElement;
        this.canvas = document.createElement('canvas');
        this.canvas.width = DEFAULT_WIDTH;
        this.canvas.height = DEFAULT_HEIGHT;

        this.visibility = "visible";

        this.canvas.style.letterSpacing = '0px';

        this.wrapper.appendChild(this.canvas);

        this.renderContext = this.canvas.getContext('2d')!;
    }

    public setVisibility(value: CommentVisibility) {

        this.visibility = value;
        this.previousRenderedTime = -1;
    }

    public clearComment(): void {

        this.layers = undefined;
    }

    public initialize(obj: any): void {

        // レイヤー順にソートする
        this.layers = obj.layers.sort((a: any, b: any) => a.index < b.index ? 1 : -1);

        // ウィンドウのサイズに合わせてスケールする
        window.addEventListener('resize', e => {

            this.calcBounds();
            this.render(this.previousRenderedTime, true);
        });

        this.layers?.forEach(layer => {
            layer.activeComments = [];
            // ベースフォントサイズを先に計算しておく
            layer.entries.forEach(entry => {

                entry.content = entry.content.replace(/\t/g, "  ");

                const scaleY = this.scaleY(entry);

                entry.baseFontSize = this.getBaseFontSize(entry);
                entry.baseFontProperty = this.getFont(entry, scaleY);
                entry.baseLineHeight = this.getBaseLineHeight(entry);

                if (entry.fontFamily === 'defont') {
                    entry.adjustYcoord = 0.01;
                } else if (entry.fontFamily === 'gothic') {
                    entry.adjustYcoord = -0.04;
                } else if (entry.fontFamily === 'mincho') {
                    entry.adjustYcoord = -0.01;
                }

                this.renderContext.font = entry.baseFontProperty;

                // 改行も込みでデフォルトウィンドウサイズにおける実際の横幅を取得する
                entry.virtualWidth = Math.max.apply(null, entry.content.split('\n').map(m => Math.ceil(this.renderContext.measureText(m).width)));
                entry.virtualHeight = entry.baseLineHeight * entry.lineCount;

                if (entry.position !== "naka") {

                    let scaleX = 1;
                    if (entry.full) {
                        if (entry.virtualWidth > DEFAULT_WIDTH_169) {
                            scaleX = DEFAULT_WIDTH_169 / entry.virtualWidth;
                        }
                    } else {
                        if (entry.virtualWidth > DEFAULT_WIDTH_43) {
                            scaleX = DEFAULT_WIDTH_43 / entry.virtualWidth;
                        }
                    }
                    if (scaleX !== 1) {
                        entry.virtualWidth *= scaleX;
                        entry.virtualHeight *= scaleX;
                        entry.baseFontSize *= scaleX;
                        entry.baseLineHeight *= scaleX;
                        entry.baseFontProperty = this.getFont(entry, scaleX * scaleY);
                    }
                }
            });
        });
        this.calcBounds();
        this.previousRenderedTime = -1;
    }

    /**
     * サイズ類を計算する
     */
    calcBounds(): void {

        this.canvas.width = this.canvas.clientWidth;
        this.canvas.height = this.canvas.clientHeight;

        // 4:3 時の横幅を求めてコメントが流れ始めるべき座標を計算する
        if (this.canvas.width > this.canvas.height) {

            this.width43 = this.canvas.height / 3 * 4;
            this.width169 = this.canvas.height / 9 * 16;
        } else {
            this.width43 = this.canvas.width / 4 * 3;
            this.width169 = this.canvas.width;
        }
        this.endLine = (this.canvas.width - this.width43) / 2;

        // 中央の位置
        this.centerLine = this.canvas.width / 2;
        this.firstLine = this.width43 / 4 * 3 + this.endLine;

        this.scalingFactor = this.canvas.height / DEFAULT_HEIGHT;

        this.wrapper.style.fontSize = `${54 * this.scalingFactor}px`;
    }

    /**
     * コメントを描画する
     * @param currentTime 再生位置 秒
     * @param forceRender 強制的に再描画させるかどうか
     */
    public render(currentTime: number, forceRender: boolean = false): void {

        if (this.layers === null) {
            return;
        }

        // コメントが非表示だった場合
        if (this.visibility === "hidden") {
            this.renderContext.clearRect(0, 0, this.canvas.width, this.canvas.height);
            return;
        }

        // 再生位置をvposと同じセンチ秒に変換する
        const centiTime = Math.floor(currentTime * 100);

        // 前回描画時と同じ時間だったらCPUリソースの無駄なので再描画しない
        if (this.previousRenderedTime === centiTime && !forceRender) {
            return;
        }

        this.renderContext.lineJoin = 'round';
        this.renderContext.clearRect(0, 0, this.canvas.width, this.canvas.height);

        /*this.renderContext.strokeStyle = 'red';
        this.renderContext.lineWidth = 2.5;
        this.renderContext.strokeRect((this.canvas.width - this.width43) / 2, 0, this.width43, this.canvas.height);
        this.renderContext.strokeStyle = 'red';
        this.renderContext.lineWidth = 2.5;
        this.renderContext.strokeRect((this.canvas.width - this.width43) / 2, 0, this.width43, this.canvas.height);
        this.renderContext.strokeStyle = 'purple';
        this.renderContext.lineWidth = 2.5;
        this.renderContext.strokeRect((this.canvas.width - this.width169) / 2, 0, this.width169, this.canvas.height);*/

        this.layers?.forEach(layer => this.renderLayer(layer, centiTime));
        this.previousRenderedTime = centiTime;
    }

    /**
     * レイヤーごとに描画する
     * @param layer コメントレイヤー
     * @param currentTime 再生位置 センチ秒
     */
    renderLayer(layer: CommentLayer, currentTime: number): void {

        // 投稿者コメント以外はレンダリングしない
        if (this.visibility === "onlyAuthor" && layer.index === 1) {
            return;
        }

        for (let entry of layer.entries) {

            if (entry.position !== "naka") {

                if (currentTime < entry.vpos || currentTime > entry.vpos + (entry.duration * 100)) {

                    if (layer.activeComments.includes(entry)) {
                        layer.activeComments = layer.activeComments.filter(f => f !== entry);
                    }
                    continue;
                }
            } else {
                if (currentTime < entry.vpos) {

                    if (layer.activeComments.includes(entry)) {
                        layer.activeComments = layer.activeComments.filter(f => f !== entry);
                    }
                }
                if (currentTime > entry.vpos + 400) {

                    if (layer.activeComments.includes(entry)) {
                        layer.activeComments = layer.activeComments.filter(f => f !== entry);
                    }
                    continue;
                }
            }

            if (!layer.activeComments.includes(entry)) {

                layer.activeComments.push(entry);
            }
        }
        layer.activeComments.forEach(f => this.renderComment(layer, currentTime, f));
    }
    /**
     * コメントを描画する
     * @param currentTime 再生位置 センチ秒
     * @param entry コメント情報
     */
    renderComment(layer: CommentLayer, currentTime: number, entry: CommentEntry): void {

        this.renderContext.fillStyle = entry.color;
        this.renderContext.textAlign = 'start';
        this.renderContext.textBaseline = 'middle';
        this.renderContext.strokeStyle = 'rgba(0,0,0,0.4)';
        this.renderContext.lineWidth = 5;
        this.renderContext.font = entry.baseFontProperty;

        // 改行も込みで正しい実際の横幅を取得する
        entry.actualWidth = entry.virtualWidth * this.scalingFactor;
        entry.actualHeight = entry.virtualHeight * this.scalingFactor;

        switch (entry.position) {
            case "naka": {

                entry.currentX = this.getX(entry, currentTime);
                entry.currentY = this.getY(entry, layer, currentTime);

                // 1行ずつ描画する
                entry.content.split('\n').forEach((content, i) => {
                    const factor = ((entry.currentY) + ((entry.baseLineHeight) * (i + 1)) - entry.baseFontSize / 2) * this.scalingFactor;
                    this.renderText(content, entry.currentX, factor);
                });

                this.renderContext.strokeStyle = 'yellow';
                this.renderContext.lineWidth = 2.5;
                //this.renderContext.strokeRect(entry.currentX, entry.currentY * this.scalingFactor, entry.actualWidth, entry.actualHeight);
                break;
            }
            case "ue": {

                entry.currentX = this.getX(entry, currentTime);
                entry.currentY = this.getY(entry, layer, currentTime);

                // 1行ずつ描画する
                entry.content.split('\n').forEach((content, i) => {
                    // 描画位置(currentY) + フォントサイズ * 固定コメント拡大率 * 行番号 * 画面スケール
                    const factor = ((entry.currentY) + ((entry.baseLineHeight) * (i + 1)) - entry.baseFontSize / 2) * this.scalingFactor;
                    this.renderText(content, entry.currentX, factor);
                });

                this.renderContext.strokeStyle = 'yellow';
                this.renderContext.lineWidth = 1;
                //this.renderContext.strokeRect(entry.currentX, entry.currentY * this.scalingFactor, entry.actualWidth, entry.actualHeight);
                break;
            }
            case "shita": {

                entry.currentX = this.getX(entry, currentTime);
                entry.currentY = this.getY(entry, layer, currentTime);

                // 1行ずつ描画する
                entry.content.split('\n').forEach((content, i) => {
                    // 描画位置(currentY) + フォントサイズ * 固定コメント拡大率 * 行番号 * 画面スケール
                    const factor = ((entry.currentY) + ((entry.baseLineHeight) * (i + 1)) - entry.baseFontSize / 2) * this.scalingFactor;
                    this.renderText(content, entry.currentX, factor);
                });

                this.renderContext.strokeStyle = 'yellow';
                this.renderContext.lineWidth = 2.5;
                //this.renderContext.strokeRect(entry.currentX, entry.currentY * this.scalingFactor, entry.actualWidth, entry.actualHeight);
                break;
            }
        }
    }

    renderText(text: string, x: number, y: number): void {

        this.renderContext.strokeText(text, x, y);
        this.renderContext.fillText(text, x, y);
    }

    getFont(entry: CommentEntry, scaleFactor: number): string {

        let fontSize = COMMENT_SIZE_NORMAL;
        if (entry.fontSize == "small") {
            fontSize = COMMENT_SIZE_SMALL;
        } else if (entry.fontSize == "big") {
            fontSize = COMMENT_SIZE_BIG;
        }

        if (entry.fontFamily === "defont") {

            return `600 ${fontSize * scaleFactor}em ${defont}`;
        } else if (entry.fontFamily === "gothic") {

            return `400 ${fontSize * scaleFactor}em ${gothic}`;
        } else if (entry.fontFamily === "mincho") {

            return `400 ${fontSize * scaleFactor}em ${mincho}`;
        } else {
            return '';
        }
    }

    /**
 * 固定コメントの高さがオーバーフローしている時は縮小するように縮小率を返す
 * @param entry 対象コメント
 */
    scaleY(entry: CommentEntry): number {

        if (!entry.ender) {

            if (entry.fontSize === "small" && entry.lineCount >= 7) {
                return 0.5526315789473684;
            }
            if (entry.fontSize === "medium" && entry.lineCount >= 5) {
                return 0.515748031496063;
            }
            if (entry.fontSize === "big" && entry.lineCount >= 3) {
                return 0.525;
            }
        }
        return 1;
    }

    getPixelPerCentiSeconds(entry: CommentEntry): number {

        // このコメントが1秒間に何ピクセル移動するべきか
        const pixelPerSeconds = (entry.actualWidth / 4) + (this.width43 / 4);
        // センチ秒に直す
        return pixelPerSeconds / 100;
    }

    getBaseFontSize(entry: CommentEntry): number {

        if (entry.fontSize === "small") {
            return 39;
        }
        if (entry.fontSize === "medium") {
            return 54;
        }
        if (entry.fontSize === "big") {
            return 70;
        }
        return 0;
    }
    getBaseLineHeight(entry: CommentEntry): number {

        if (entry.lineCount === 1) {

            if (entry.fontSize === "small") {
                return DEFAULT_HEIGHT / 16.6;
            }
            if (entry.fontSize === "medium") {
                return DEFAULT_HEIGHT / 11.3;
            }
            if (entry.fontSize === "big") {
                return DEFAULT_HEIGHT / 7.8;
            }
        }

        if (entry.ender) {

            if (entry.fontSize === "small") {
                return DEFAULT_HEIGHT / 21;
            }
            if (entry.fontSize === "medium") {
                return DEFAULT_HEIGHT / 13.1;
            }
            if (entry.fontSize === "big") {
                return DEFAULT_HEIGHT / 8.4;
            }
        } else {

            if (entry.fontSize === "small" && entry.lineCount >= 7) {
                //return (DEFAULT_HEIGHT - (16.6 * (21 / 38))) / 38;
                return DEFAULT_HEIGHT / 38;
            }
            if (entry.fontSize === "medium" && entry.lineCount >= 5) {
                //return (DEFAULT_HEIGHT - (11.3 * (13.1 / 25.4))) / 25.4;
                return DEFAULT_HEIGHT / 25.4;
            }
            if (entry.fontSize === "big" && entry.lineCount >= 3) {
                //return (DEFAULT_HEIGHT - (7.8 * (8.4 / 16))) / 16;
                return DEFAULT_HEIGHT / 16;
            }
        }

        return 0;
    }

    /**
     * 対象のコメントが指定した再生時間時に画面上に居るべきX座標を返す
     * この値は実際のサイズの値を返す getYとは違う
     * @param entry コメント
     * @param currentTime 現在時間など
     */
    getX(entry: CommentEntry, currentTime: number): number {

        switch (entry.position) {
            case "naka": {

                // 再生時間とコメント表示時間のずれ
                const diffCenti = currentTime - entry.vpos;

                // このコメントが1秒間に何ピクセル移動するべきか
                const pPcs = this.getPixelPerCentiSeconds(entry);
                // nセンチ秒 * 1センチ秒に移動するピクセル数 + オフセット
                const diff = (diffCenti * pPcs);

                return this.firstLine - diff - (entry.actualWidth / 4);
            }
            case "ue":
            case "shita": {

                if (entry.full) {
                    if (this.width169 <= entry.actualWidth) {
                        return (this.canvas.width - this.width169) / 2;
                    }
                } else {
                    if (this.width43 <= entry.actualWidth) {
                        return this.endLine;
                    }
                }
                // ウィンドウからはみ出していないのでウィンドウの横幅からコメントの横幅を引いて2で割った値を返す
                return (this.canvas.width - entry.actualWidth) / 2;
            }
        }
    }


    /**
     * 対象のコメントが表示するべきY座標を返す
     * ここでは1366 * 768サイズ前提で値が返るので実際に使う際は拡大率を掛けて使う
     * @param entry 対象のコメント
     * @param activeComments 現在画面上に表示されているコメントのリスト
     */
    getY(entry: CommentEntry, layer: CommentLayer, currentTime: number): number {

        // Y座標が既に決まっているコメントはその値を使う
        if (entry.currentY !== undefined) {
            return entry.currentY;
        }

        // 自分以外の描画したいコメントと同じ位置に既に描画されているコメントのリスト
        const activeSamePositionComments = layer.activeComments.filter(f => f.no !== entry.no && f.position === entry.position && !f.isFreedomYcoord).sort((a, b) => {

            if (a.currentY < b.currentY) return -1;
            if (a.currentY > b.currentY) return 1;
            if (a.vpos < b.vpos) return -1;
            if (a.vpos > b.vpos) return 1;
            return 0;
        });

        switch (entry.position) {
            case "naka": {
                // 高さ固定コメントの時は中央揃えになるようにする
                if (entry.ender) {

                    if ((entry.fontSize === "big" && entry.lineCount >= 8) ||
                        (entry.fontSize === "medium" && entry.lineCount >= 13) ||
                        (entry.fontSize === "small" && entry.lineCount >= 21)) {
                        entry.isFreedomYcoord = true;
                        return (DEFAULT_HEIGHT - entry.virtualHeight) / 2;
                    }
                } else {

                    if ((entry.fontSize === "big" && entry.lineCount >= 16) ||
                        (entry.fontSize === "medium" && entry.lineCount >= 25) ||
                        (entry.fontSize === "small" && entry.lineCount >= 38)) {
                        entry.isFreedomYcoord = true;
                        return (DEFAULT_HEIGHT - entry.virtualHeight) / 2;
                    }
                }

                // 何も描画されていない時
                if (activeSamePositionComments.length === 0) {

                    return 0;
                }

                // ピクセル per センチ秒
                const pPcs = this.getPixelPerCentiSeconds(entry);
                // 対象のコメントの先頭部分がendLineまで到達するまでにかかるセンチ秒
                const offset = (this.getX(entry, entry.vpos) - this.endLine) / pPcs;

                let yCandidate = 0;
                let flag = false;
                do {
                    flag = false;

                    let i: any;
                    for (i in activeSamePositionComments) {
                        const f = activeSamePositionComments[i];

                        // 候補のY座標が現在描画されているコメントのY座標＋コメントの高さより小さかったら
                        if (f.currentY + f.virtualHeight > yCandidate) {
                            if (yCandidate + entry.virtualHeight > f.currentY) {

                                // 既に描画されているコメントの初回描画位置(x座標)
                                const startX = this.getX(f, f.vpos) + f.actualWidth;
                                // 既に描画されているコメントの時間の時に描画したいコメントがどの位置に来るかを調べる
                                const targetStartX = this.getX(entry, f.vpos);

                                // endLine到達直後のx座標を調べて対象のコメントが重なるかどうかを判定する
                                const x = this.getX(f, entry.vpos + offset) + f.actualWidth;

                                // 描画対象のコメントがendLineに到達した時間に同じ行にいる既存のコメントがendLineを超えていればその行にはコメント可能
                                // かつ、描画開始時に既存のコメントよりも描画対象のコメントが少しでも前に出ていたら重なってしまうのでその行にはコメント不可
                                if (!(x < this.endLine && startX < targetStartX)) {
                                    yCandidate = f.currentY + f.virtualHeight;
                                    if (yCandidate + entry.virtualHeight > DEFAULT_HEIGHT) {

                                        entry.isFreedomYcoord = true;
                                        let rand = Math.random() * DEFAULT_HEIGHT;
                                        if (rand > entry.virtualHeight) {
                                            rand -= entry.virtualHeight;
                                        }
                                        return rand;
                                    }
                                    flag = true;
                                    break;
                                }
                            }
                        }

                    }

                } while (flag);

                return yCandidate;
            }
            case "ue": {

                if (entry.ender) {

                    if ((entry.fontSize === "big" && entry.lineCount >= 8) ||
                        (entry.fontSize === "medium" && entry.lineCount >= 13) ||
                        (entry.fontSize === "small" && entry.lineCount >= 21)) {
                        entry.isFreedomYcoord = true;
                        return 0;
                    }
                } else {

                    if ((entry.fontSize === "big" && entry.lineCount >= 16) ||
                        (entry.fontSize === "medium" && entry.lineCount >= 25) ||
                        (entry.fontSize === "small" && entry.lineCount >= 38)) {
                        entry.isFreedomYcoord = true;
                        return 0;
                    }
                }

                // 何も描画されていない時
                if (activeSamePositionComments.length === 0) {
                    // 1番上に描画する
                    return 0;
                }

                for (let i in activeSamePositionComments) {
                    const f = activeSamePositionComments[i];

                    // 一番上のコメント(i === 0)でそのコメントが一番上に張り付いていない場合に入る余地があるか確認する
                    if (Number(i) === 0 && f.currentY > entry.virtualHeight) {
                        // 1番上に描画する
                        return 0;
                    }

                    // 現在描画されているコメントの位置から描画したいコメントの高さを足した場所に描画したい
                    const requestStart = f.currentY + f.virtualHeight;
                    const requestEnd = requestStart + entry.virtualHeight;

                    // 次のコメントがあれば現在描画されているコメントと位置を比較して隙間に入る余地があるか確認する
                    if (activeSamePositionComments.length > Number(i) + 1) {

                        const nextEntry = activeSamePositionComments[Number(i) + 1];
                        const nextEnd = nextEntry.currentY + nextEntry.virtualHeight;
                        if (requestEnd !== nextEnd) {
                            return requestStart;
                        }
                    } else {
                        // 上コメがオーバーフローしていたら位置をランダムな場所にする
                        if (requestEnd > DEFAULT_HEIGHT) {

                            entry.isFreedomYcoord = true;
                            let rand = Math.random() * DEFAULT_HEIGHT;
                            if (rand > entry.virtualHeight) {
                                rand -= entry.virtualHeight;
                            }
                            return rand;
                        }
                        return requestStart;
                    }


                }

                return 0;
                break;
            }
            case "shita": {

                if (entry.ender) {

                    if ((entry.fontSize === "big" && entry.lineCount >= 8) ||
                        (entry.fontSize === "medium" && entry.lineCount >= 13) ||
                        (entry.fontSize === "small" && entry.lineCount >= 21)) {
                        entry.isFreedomYcoord = true;
                        return DEFAULT_HEIGHT - entry.virtualHeight;
                    }
                } else {

                    if ((entry.fontSize === "big" && entry.lineCount >= 16) ||
                        (entry.fontSize === "medium" && entry.lineCount >= 25) ||
                        (entry.fontSize === "small" && entry.lineCount >= 38)) {
                        entry.isFreedomYcoord = true;
                        return DEFAULT_HEIGHT - entry.virtualHeight;
                    }
                }

                if (activeSamePositionComments.length === 0) {
                    // 1番下に描画する
                    return DEFAULT_HEIGHT - entry.virtualHeight;
                }
                // shitaなので逆から参照する
                const reversed = activeSamePositionComments.reverse();
                for (let i in reversed) {
                    const f = reversed[i];

                    // 一番下のコメント(i === 0)でそのコメントが一番下に張り付いていない場合に入る余地があるか確認する
                    if (Number(i) === 0 && DEFAULT_HEIGHT - (f.currentY + f.virtualHeight) >= entry.virtualHeight) {

                        // 1番下に描画する
                        return DEFAULT_HEIGHT - entry.virtualHeight;
                    }

                    // 現在描画されているコメントの位置から描画したいコメントの高さを引いた場所に描画したい
                    const requestStart = f.currentY - entry.virtualHeight;
                    const requestEnd = f.currentY;

                    // 次のコメントがあれば現在描画されているコメントと位置を比較して隙間に入る余地があるか確認する
                    if (reversed.length > Number(i) + 1) {

                        const nextEntry = reversed[Number(i) + 1];
                        const nextEnd = nextEntry.currentY + nextEntry.virtualHeight;
                        if (requestEnd !== nextEnd) {
                            return requestStart;
                        }
                    } else {
                        // 下コメがオーバーフローしていたら位置をランダムな場所にする
                        if (requestStart < 0) {

                            entry.isFreedomYcoord = true;
                            let rand = Math.random() * DEFAULT_HEIGHT;
                            if (rand > entry.virtualHeight) {
                                rand -= entry.virtualHeight;
                            }
                            return rand;
                        }
                        return requestStart;
                    }
                }
                return DEFAULT_HEIGHT - entry.virtualHeight;
            }
        }
    }
};


