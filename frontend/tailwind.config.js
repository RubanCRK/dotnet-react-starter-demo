/** @type {import('tailwindcss').Config} */
export default {
  // Class strategy so the saved user theme preference controls dark styling
  darkMode: 'class',
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        brand: {
          header: 'var(--color-brand-header)',
          content: 'var(--color-brand-content)',
          primary: 'var(--color-brand-primary)',
          text: 'var(--color-brand-text)',
        },
      },
    },
  },
  plugins: [],
};
