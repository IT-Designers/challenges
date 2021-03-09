module.exports = {
    env: {
        browser: true
    },
    extends:
        "airbnb-base", //If you think a rule is stupid read her why they are like this: https://github.com/airbnb/javascript
    globals: {
        Atomics: "readonly",
        SharedArrayBuffer: "readonly"
    },
    parserOptions: {
        ecmaVersion: 2020,
        sourceType: "module"
    },
    rules: {
        //Most people develop on windows
        "linebreak-style": ["off", "windows"],
        //Some rules are changed to make the codestyle more equal to c#
        indent: ["error", 4],
        quotes: ["error", "double"],
        "quote-props": ["error", "as-needed"],
        "comma-dangle": ["error", "never"],
        "nonblock-statement-body-position": ["error", "beside"],
        "spaced-comment": ["error", "never"],
        "no-use-before-define": ["error", { functions: false }],
        "no-plusplus": ["error", { allowForLoopAfterthoughts: true }],
        "object-shorthand": ["error", "never"],

        "prefer-arrow-callback": "off", //Because of methodbinding in vue.js this needs to be deactived
        "prefer-destructuring": "off", //Destructing of objects are not well known and are therefore not enforced
        "no-restricted-globals": ["off", "confirm"], //Its okay
        "max-len": ["off", { code: 160 }]
    }
};
