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
- make sure to have a consistent style. Colour palette is under constants folder.

Try to group related move files together to speed up the development process.

## Project Structure Best Practices

Documentation is found at [docs](./docs/*)

File structure can be found by running `tree /Users/tom/Code/NetWorth/app -I "node_modules"`

- When adding new features or code, organize files into the correct folders according to best practices and the existing file structure.
- Only create folders as needed to keep the project organized and maintainable.
- Always update imports when moving files to new folders.
- Use clear naming conventions and keep related files together.

## Note

- If you are unsure where a new file should go, prefer creating a new folder following the above structure.
- Keep this file updated as the project evolves.


Ensure you write tests to programatically assert major changes or where you see gaps.