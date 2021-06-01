
const DEFAULT_WIDTH = 1366;
const DEFAULT_HEIGHT = 768;

// font-size: 54の場合
const COMMENT_SIZE_NORMAL = '1em';
const COMMENT_SIZE_SMALL = '0.69em';
const COMMENT_SIZE_BIG = '1.425em';

const defont: string = 'Arial, "ＭＳ Ｐゴシック", "MS PGothic", MSPGothic, MS-PGothic';
const gothic: string = '游ゴシック体';
const mincho: string = '游明朝体';

export class CommentHandler {

    wrapper: HTMLDivElement;
    canvas: HTMLCanvasElement;
    renderContext: CanvasRenderingContext2D;

    layers: CommentLayer[] | undefined;

    previousRenderedTime: number = 0;

    startLine: number = DEFAULT_HEIGHT / 3 * 4;
    endLine: number = 0;
    centerLine: number = 0;
    firstLine: number = 0;
    width43: number = 0;

    scalingRate: number = 0;

    constructor() {

        this.wrapper = document.getElementById('comment') as HTMLDivElement;
        this.canvas = document.createElement('canvas');
        this.canvas.width = DEFAULT_WIDTH;
        this.canvas.height = DEFAULT_HEIGHT;

        this.canvas.style.letterSpacing = '0px';

        this.wrapper.appendChild(this.canvas);

        this.renderContext = this.canvas.getContext('2d')!;
    }

    public initialize(obj: any): void {

        this.layers = obj.layers;

        // ウィンドウのサイズに合わせてスケールする
        window.addEventListener('resize', e => {

            this.calcBounds();
            this.render(this.previousRenderedTime);
        });

        this.layers?.forEach(layer => layer.activeComments = []);
        this.calcBounds();
    }

    /**
     * サイズ類を計算する
     */
    calcBounds(): void {

        this.canvas.width = this.canvas.clientWidth;
        this.canvas.height = this.canvas.clientHeight;

        // 4:3 時の横幅を求めてコメントが始めるべき座標を計算する
        this.width43 = this.canvas.height / 3 * 4;
        this.endLine = (this.canvas.width - this.width43) / 2;
        this.startLine = this.endLine + this.width43;

        // 中央の位置
        this.centerLine = this.canvas.width / 2;
        this.firstLine = this.width43 / 4 * 3 + this.endLine;

        this.scalingRate = this.canvas.height /DEFAULT_HEIGHT;

        this.wrapper.style.fontSize = `${54 * this.scalingRate}px`;
    }

    /**
     * コメントを描画する
     * @param currentTime 再生位置 秒
     */
    public render(currentTime: number): void {

        if (this.layers === null) {
            return;
        }

        // 再生位置をvposと同じセンチ秒に変換する
        const centiTime = Math.floor(currentTime * 100);

        // 前回描画時と同じ時間だったらCPUリソースの無駄なので再描画しない
        if (this.previousRenderedTime === centiTime) {
            //return;
        }

        this.renderContext.clearRect(0, 0, this.canvas.width, this.canvas.height);
        this.renderContext.textAlign = 'start';
        this.renderContext.lineJoin = 'miter';

        this.layers?.forEach(layer => this.renderLayer(layer, centiTime));
        this.previousRenderedTime = centiTime;
    }

    /**
     * レイヤーごとに描画する
     * @param layer コメントレイヤー
     * @param currentTime 再生位置 センチ秒
     */
    renderLayer(layer: CommentLayer, currentTime: number): void {

        for (var index in layer.entries) {
            const entry = layer.entries[index];

            // 描画が始まる時間 - 1000センチ秒になってないのでcontinue
            if (currentTime < entry.vpos - 1000) {

                if (layer.activeComments.includes(entry)) {
                    //layer.activeComments = layer.activeComments.filter(f => f !== entry);
                }
            }
            if (currentTime > entry.vpos + 400) {

                if (layer.activeComments.includes(entry)) {
                    //layer.activeComments = layer.activeComments.filter(f => f !== entry);
                }
                //continue;
            }

            this.renderComment(layer, currentTime, entry);

            if (!layer.activeComments.includes(entry)) {

                layer.activeComments.push(entry);
            }
        }
    }
    /**
     * コメントを描画する
     * @param currentTime 再生位置 センチ秒
     * @param entry コメント情報
     */
    renderComment(layer: CommentLayer, currentTime: number, entry: CommentEntry): void {
         
        let fontSize = COMMENT_SIZE_NORMAL;
        if (entry.fontSize == "small") {
            fontSize = COMMENT_SIZE_SMALL;
        } else if (entry.fontSize == "big") {
            fontSize = COMMENT_SIZE_BIG;
        }

        this.renderContext.font = `600 ${fontSize} ${defont}`;
        this.renderContext.fillStyle = entry.color;
        
        this.renderContext.strokeStyle = 'gray';
        this.renderContext.lineWidth = 2.8;

        const measured = this.renderContext.measureText(entry.content);
        entry.actualWidth = measured.width;
        entry.actualHeight = this.getBaseFontSize(entry) + 11;

        switch (entry.position) {
            case "naka": {

                entry.currentX = this.getX(entry, currentTime);
                entry.currentY = this.getY(entry, layer, currentTime);

                const actualY = entry.currentY * this.scalingRate;

                this.renderContext.strokeText(entry.content, entry.currentX, actualY);
                this.renderContext.fillText(entry.content, entry.currentX, actualY);

                this.renderContext.strokeStyle = 'yellow';
                this.renderContext.lineWidth = 2.5;
                this.renderContext.strokeRect(entry.currentX, entry.currentY * this.scalingRate - this.getBaseFontSize(entry), entry.actualWidth, entry.actualHeight * this.scalingRate);
                break;
            }
            case "ue": {

                break;
            }
            case "shita": {

                break;
            }
        }
    }

    getPixelPerCentiSeconds(entry: CommentEntry): number {

        // このコメントが1秒間に何ピクセル移動するべきか
        const pixelPerSeconds = (entry.actualWidth / 4) + (this.width43 / 4);
        // センチ秒に直す
        return pixelPerSeconds / 100;
    }

    getBaseFontSize(entry: CommentEntry): number {

        if (entry.fontSize === "middle") {
            return 54;
        }
        if (entry.fontSize === "small") {
            return 38;
        }
        if (entry.fontSize === "big") {
            return 70;
        }
        return 0;
    }

    /**
     * 対象のコメントが指定した再生時間時に画面上に居るべきX座標を返す
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
            case "ue": {

                break;
            }
            case "shita": {

                break;
            }
        }
        return 0;
    }


    /**
     * 対象のコメントが表示するべきY座標を返す
     * ここでは1366 * 768サイズ前提で値が返るので実際に使う際は拡大率を掛けて使う
     * @param entry 対象のコメント
     * @param activeComments 現在画面上に表示されているコメントのリスト
     */
    getY(entry: CommentEntry, layer: CommentLayer, currentTime: number): number {

        // Y座標が既に決まっているコメントはその値を使う
        if (entry.currentY) {
            return entry.currentY;
        }

        // 自分以外の描画したいコメントと同じ位置に既に描画されているコメントのリスト
        const activeSamePositionComments = layer.activeComments.filter(f => f.no !== entry.no && f.position === entry.position).sort((a, b) => {

            if (a.currentY < b.currentY) return -1;
            if (a.currentY > b.currentY) return 1;
            if (a.vpos < b.vpos) return -1;
            if (a.vpos > b.vpos) return 1;
            return 0;
        });

        switch (entry.position) {
            case "naka": {

                // 何も描画されていない時
                if (activeSamePositionComments.length === 0) {

                    return this.getBaseFontSize(entry);
                }

                // ピクセル per センチ秒
                const pPcs = this.getPixelPerCentiSeconds(entry);
                // 対象のコメントの先頭部分がendLineまで到達するまでにかかるセンチ秒
                const offset = (this.getX(entry, entry.vpos) - this.endLine) / pPcs;

                let i: any;
                for (i in activeSamePositionComments) {

                    const f = activeSamePositionComments[i];

                    // 既に描画されているコメントの初回描画位置(x座標)
                    const startX = this.getX(f, f.vpos) + f.actualWidth;
                    // 既に描画されているコメントの時間の時に描画したいコメントがどの位置に来るかを調べる
                    const targetStartX = this.getX(entry, f.vpos);

                    // endLine到達直後のx座標を調べて対象のコメントが重なるかどうかを判定する
                    const x = this.getX(f, entry.vpos + offset) + f.actualWidth;

                    // 描画対象のコメントがendLineに到達した時間に同じ行にいる既存のコメントがendLineを超えていればその行にはコメント可能
                    // かつ、描画開始時に既存のコメントよりも描画対象のコメントが少しでも前に出ていたら重なってしまうのでその行にはコメント不可
                    if (x < this.endLine && startX < targetStartX) {

                        // 同じ行にコメントできるので既存のコメントと同じ位置に描画する
                        layer.activeComments = layer.activeComments.filter(ff => ff !== f);
                        return f.currentY / this.getBaseFontSize(f) * this.getBaseFontSize(entry);
                    }
                }
                return activeSamePositionComments[i].currentY + (this.getBaseFontSize(entry) + 11) ;
            }
            case "ue": {

                break;
            }
            case "shita": {

                break;
            }
        }

        return 54;
    }

};


