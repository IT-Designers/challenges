const path = require("path");
const webpack = require("webpack");

const assetPath = process.env.ASSET_PATH || "/dist/";

module.exports = {
    mode: "development",
    entry: {
        app: "./wwwroot/source/site.js",
        submissionView: "./wwwroot/source/submission-view.js",
        review: "./wwwroot/source/ReviewEditorFunctions.js",
        mdeditor: "./wwwroot/source/BlazorQuill.js"
    },
    output: {
        path: path.resolve(__dirname, "wwwroot/dist"),
        publicPath: assetPath,
        filename: "[name].bundle.js",
        libraryTarget: "var",
        library: "Notification"
    },
    module: {
        rules: [
            {
                test: /\.css$/,
                use: [
                    { loader: "style-loader" },
                    { loader: "css-loader" }
                ]
            },
            {
                test: /\.(png|woff|woff2|eot|ttf|svg)$/,
                use: [
                    { loader: "url-loader?limit=100000" }
                ]
            },
            {
                test: /\.(gif|png|jpe?g|svg)$/,
                use: [
                    { loader: "file-loader" }
                ]
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: [
                    {
                        loader: "eslint-loader",
                        options: {
                            //eslint options (if necessary)
                        }
                    }
                ]

            }
        ]
    },
    plugins: [
        new webpack.ProvidePlugin({
            //Is needed or jstree in review/review.js
            jQuery: "jquery"
        }),
        new webpack.DefinePlugin({
            "process.env.ASSET_PATH": JSON.stringify(assetPath)
        })
    ]
};
