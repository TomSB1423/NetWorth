// Event-based navigation solution
import { EventRegister } from 'react-native-event-listeners';

export const NAVIGATION_EVENTS = {
  ACCOUNT_CREATED: 'ACCOUNT_CREATED',
  NAVIGATE_TO_DASHBOARD: 'NAVIGATE_TO_DASHBOARD',
};

export const emitAccountCreated = () => {
  EventRegister.emit(NAVIGATION_EVENTS.ACCOUNT_CREATED);
};

export const emitNavigateToDashboard = () => {
  EventRegister.emit(NAVIGATION_EVENTS.NAVIGATE_TO_DASHBOARD);
};

export const listenForAccountCreated = (callback: () => void) => {
  const listener = EventRegister.addEventListener(NAVIGATION_EVENTS.ACCOUNT_CREATED, callback);
  return () => EventRegister.removeEventListener(listener);
};

export const listenForNavigateToDashboard = (callback: () => void) => {
  const listener = EventRegister.addEventListener(NAVIGATION_EVENTS.NAVIGATE_TO_DASHBOARD, callback);
  return () => EventRegister.removeEventListener(listener);
};
