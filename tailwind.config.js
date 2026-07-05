/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,jsx}"
  ],
  theme: {
    extend: {
      screen:{
        xs:'500px',
      },
    },
  },
  plugins: [],
}