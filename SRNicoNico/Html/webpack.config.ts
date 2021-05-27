import * as webpack from 'webpack';

const config: webpack.Configuration = {
    mode: 'production',
    entry: ['./Html\\videolayer.ts', './Html\\commentlayer.ts'],
    output: {
        path: __dirname,
        filename: 'player.bundle.js',
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                use: 'ts-loader'
            }, {
                test: /\.scss$/,
                use: [
                    'style-loader',
                    {
                        loader: 'css-loader',
                        options: {
                            sourceMap: false,
                            importLoaders: 2
                        }
                    },
                    {
                        loader: 'sass-loader',
                    }
                ]
            }
        ]
    }
};

export default config;