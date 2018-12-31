const merge = require("webpack-merge");
const config = require("./webpack.config.js");

module.exports = merge(config, {

    mode: "production",
    output: {
        path: `${__dirname}/bin/Release/Html`
    }
});