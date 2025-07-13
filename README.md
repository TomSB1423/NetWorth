# NetWorth 💰

A comprehensive personal finance and net worth tracking mobile application built with React Native and Expo. Track your financial accounts, monitor your net worth over time, and visualize your financial progress with beautiful charts and analytics.

## Features ✨

- **Dashboard Overview**: Real-time net worth tracking with interactive charts
- **Account Management**: Add and manage multiple financial accounts (checking, savings, investments)
- **Transaction Tracking**: Monitor your financial transactions across all accounts
- **Visual Analytics**: Monthly flow charts, category breakdowns, and net worth history
- **Multi-platform**: Runs on iOS, Android, and Web
- **Theme Support**: Light and dark mode support
- **Onboarding**: Guided setup for new users

## Technology Stack 🛠️

- **React Native** with **Expo** for cross-platform development
- **TypeScript** for type safety
- **Redux Toolkit** for state management
- **Expo Router** for file-based navigation
- **React Native Chart Kit** for data visualization
- **Jest** for testing

## Installation Instructions 📱

### Prerequisites

- Node.js (version 18 or higher)
- npm or yarn package manager
- Expo CLI (for development)

### Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/TomSB1423/NetWorth.git
   cd NetWorth
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start the development server**
   ```bash
   npm start
   # or
   npx expo start
   ```

4. **Run on your device**
   
   After starting the development server, you can run the app on:
   
   - **iOS Simulator**: Press `i` in the terminal or scan the QR code with the Camera app
   - **Android Emulator**: Press `a` in the terminal or scan the QR code with the Expo Go app
   - **Physical Device**: Install [Expo Go](https://expo.dev/go) and scan the QR code
   - **Web Browser**: Press `w` in the terminal to open in your browser

### Development Commands

```bash
# Start the app
npm start

# Run on specific platforms
npm run android    # Android emulator
npm run ios        # iOS simulator
npm run web        # Web browser

# Testing and code quality
npm test           # Run tests
npm run lint       # Run ESLint
```

### Building for Production

```bash
# Create production build
npx expo build

# Or use EAS Build for more advanced builds
npx eas build --platform all
```

## Project Structure 📁

```
NetWorth/
├── app/                    # Main application screens (file-based routing)
│   ├── (tabs)/            # Tab navigation screens
│   │   ├── dashboard.tsx  # Net worth dashboard
│   │   ├── transactions.tsx # Account management
│   │   └── settings.tsx   # App settings
│   ├── onboarding/        # User onboarding flow
│   └── index.tsx          # App entry point
├── components/            # Reusable UI components
├── hooks/                # Custom React hooks
├── store/                # Redux store and slices
├── services/             # API and data services
├── constants/            # App constants and themes
└── types/                # TypeScript type definitions
```

## Getting Started 🚀

1. **First Launch**: The app will guide you through an onboarding process
2. **Add Accounts**: Start by adding your financial accounts (checking, savings, etc.)
3. **Track Net Worth**: View your dashboard to see real-time net worth calculations
4. **Monitor Progress**: Use the charts and analytics to track your financial progress over time

## Contributing 🤝

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License 📄

This project is private and proprietary.

---

Built with ❤️ using [Expo](https://expo.dev) and [React Native](https://reactnative.dev)
