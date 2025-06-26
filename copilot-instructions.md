# GitHub Copilot Instructions

- Use clear and concise code.
- Follow the project's existing coding style and React Native best practices.
- Use functional components and React Hooks.
- Add comments to explain complex logic and component behavior.
- Write functions and components with descriptive names.
- Prefer readability and maintainability over cleverness.
- Include error handling where appropriate.
- Use TypeScript for type safety if possible.
- Organize components, screens, and utilities in separate folders.
- Ensure UI is accessible and responsive on different devices.
- Use PropTypes or TypeScript interfaces for component props.
- Avoid inline styles; use StyleSheet for styling.

## Project Structure Best Practices

- When adding new features or code, organize files into the following folders as appropriate:
  - `components`: For reusable UI components.
  - `screens`: For top-level screens/pages.
  - `services`: For business logic and data access (already in use).
  - `hooks`: For custom React hooks.
  - `utils` or `lib`: For utility/helper functions.
  - `types` or `models`: For TypeScript types and interfaces.
  - `constants`: For app-wide constants.
  - `navigation`: For navigation setup.
- Only create folders as needed to keep the project organized and maintainable.
- Always update imports when moving files to new folders.
- Use clear naming conventions and keep related files together.

## Expo Router File/Folder Best Practices

- Expo Router treats every file and folder in the root of the `app/` directory as a route by default.
- To avoid non-screen folders (like `components`, `constants`, `hooks`, etc.) appearing as tabs or routes:
  - Move these folders out of the root of `app/` (e.g., to the project root), or
  - Prefix them with an underscore (e.g., `_components`, `_constants`).
- Only keep actual screen files or folders (e.g., `index.tsx`, `transactions.tsx`) at the root of `app/`.
- This keeps navigation clean and prevents unwanted tabs or routes from being generated.

## Note

- If you are unsure where a new file should go, prefer creating a new folder following the above structure.
- Keep this file updated as the project evolves.
