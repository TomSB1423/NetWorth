/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {
            animation: {
                'spin-slow': 'spin 3s linear infinite',
                'spin-reverse': 'spin-reverse 4s linear infinite',
                'bounce-gentle': 'bounce-gentle 2s ease-in-out infinite',
                'float': 'float 4s ease-in-out infinite',
                'fade-in': 'fade-in 0.5s ease-out forwards',
            },
            keyframes: {
                'spin-reverse': {
                    from: { transform: 'rotate(360deg)' },
                    to: { transform: 'rotate(0deg)' },
                },
                'bounce-gentle': {
                    '0%, 100%': { transform: 'translateY(0)' },
                    '50%': { transform: 'translateY(-4px)' },
                },
                'float': {
                    '0%, 100%': { transform: 'translateY(0) opacity(0.4)' },
                    '50%': { transform: 'translateY(-20px) opacity(1)' },
                },
                'fade-in': {
                    '0%': { opacity: '0', transform: 'translateY(5px)' },
                    '100%': { opacity: '1', transform: 'translateY(0)' },
                },
            },
        },
    },
    plugins: [],
}

