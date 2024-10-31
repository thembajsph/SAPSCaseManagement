/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Views/**/*.cshtml', // Include your views
        './Views/Shared/**/*.cshtml', // Include shared views if necessary
        './wwwroot/**/*.html', // Include any HTML files if they exist
        './wwwroot/**/*.js'
    ],
    theme: {
        extend: {},
    },
    plugins: [],
}


