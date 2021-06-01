import * as webpack from 'webpack';

const config: webpack.Configuration = {
    mode: 'production',
    entry: ['./Html\\videolayer.ts'],
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
    },
    resolve: {
        extensions: [ '.ts', '.js', '.css' ]
    }
};

export default config;