/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.{cshtml,html}',           // Razor Pages
        './Views/**/*.{cshtml,html}',           // Nếu có Views
        './wwwroot/js/**/*.js',                 // JS trong wwwroot (nếu dùng)
        './node_modules/flowbite/**/*.js',       // Flowbite components
        "./wwwroot/**/*.cshtml"
    ],
    theme: {
        extend: {},
    },
    plugins: [
        require('daisyui'),                     // DaisyUI plugin
        require('flowbite/plugin')              // Flowbite plugin
    ],
}
