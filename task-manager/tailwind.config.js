/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        "dark-blue": "#1a1f2e",
        "dark-blue-light": "#1e2536",
        "primary-blue": "#3b5bdb",
        "primary-blue-dark": "#364fc7",
        "primary-green": "#2b8a3e",
        "primary-green-dark": "#2b7a3e",
        "primary-red": "#e03131",
        "primary-red-dark": "#c92a2a",
      },
    },
  },
  plugins: [],
};
