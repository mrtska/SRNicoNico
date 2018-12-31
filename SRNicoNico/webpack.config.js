
var path = require("path");
var webpack = require('webpack');

module.exports = {
    entry: {
        "videohtml5": "Html/videohtml5.ts",
        "livehtml5": "Html/livehtml5.ts"
    },
    output: {
        filename: "[name].js"
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                loader: "ts-loader"
            },
            {
                test: /[\.png|\.gif]$/,
                loader: "url-loader"
            },
            {
                test: /\.css$/,
                use: [
                    "style-loader",
                    {
                        loader: "css-loader",
                        options: {
                            sourceMap: false,
                            importLoaders: 2
                        }
                    }
                ]
            },
            {
                test: /\.scss$/,
                use: [
                    "style-loader",
                    {
                        loader: "css-loader",
                        options: {
                            sourceMap: false,
                            importLoaders: 2
                        }
                    },
                    {
                        loader: "postcss-loader",
                        options: {
                            plugins: [
                                require('autoprefixer')({
                                    grid: true,
                                    "browsers": [
                                        "> 1%",
                                        "IE 10"
                                    ]
                                })
                            ]
                        }
                    },
                    {
                        loader: "sass-loader",
                        options: {
                            includePaths: ["Html"]
                        }
                    }
                ]
            }
        ]
    },
    resolve: {
        extensions: [".ts", ".js", ".css"],
        modules: [
            path.resolve("."),
            path.resolve("Html"),
            path.resolve("node_modules")
        ],
        alias: {
            "vue$": "vue/dist/vue.esm.js",
            "jquery": "Html/jquery-3.3.1.js"
        }
    }
}