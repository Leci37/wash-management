// @ts-check
const eslint = require("@eslint/js");
const tseslint = require("typescript-eslint");
const angular = require("angular-eslint");
const prettier = require("eslint-config-prettier");


// /** @type {import("eslint").Linter.FlatConfig[]} */
module.exports = tseslint.config(
  {
    files: ["**/*.ts"],
    extends: [
      eslint.configs.recommended,
      ...tseslint.configs.recommended,
      ...tseslint.configs.stylistic,
      ...angular.configs.tsRecommended,
      prettier
    ],
    plugins: {
      prettier: require("eslint-plugin-prettier")
    },
    processor: angular.processInlineTemplates,
    rules: {
      "prettier/prettier": ["error",  {"endOfLine": "auto"}],  // show prettier issues as ESLint errors, and solve the issue with endOfLine error 
      "@angular-eslint/directive-selector": [
        "error",
        {
          type: "attribute",
          prefix: "cm",
          style: "camelCase",
        },
      ],
      "@angular-eslint/component-selector": [
        "error",
        {
          type: "element",
          prefix: "cm",
          style: "kebab-case",
        },
      ],
    },
  },
  {
    files: ["**/*.html"],
    extends: [
      ...angular.configs.templateRecommended,
      ...angular.configs.templateAccessibility,
    ],
    rules: {},
  }
);
