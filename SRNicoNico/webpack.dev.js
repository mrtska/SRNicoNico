const merge = require("webpack-merge");
const config = require("./webpack.config.js");

module.exports = merge(config, {

    mode: "development",
    output: {
        path: `${__dirname}/bin/Debug/Html`
    }
});