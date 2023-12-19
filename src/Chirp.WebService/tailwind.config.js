module.exports = {
  purge: {
    enabled: true,
    content: [
      './Pages/**/*.cshtml',
      './Pages/Shared/*.cshtml',
      './Pages/*.cshtml',
      './Views/**/*.cshtml'
    ]
  },
  darkMode: false, // or 'media' or 'class'
  theme: {
    extend: {},
  },
  variants: {
    extend: {},
  },
  plugins: [],
}
