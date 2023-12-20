module.exports = {
  important: true,
  purge: {
    enabled: true,
    content: [
      "./node_modules/flowbite/**/*.js",
      './Pages/**/*.cshtml',
      './Pages/Shared/*.cshtml',
      './Pages/*.cshtml',
      './Views/**/*.cshtml'
    ]
  },
  darkMode: 'media', // or 'media' or 'class'
  theme: {
    extend: {},
  },
  variants: {
    extend: {},
  },
  plugins: [
  ],
}
