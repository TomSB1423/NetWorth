// Navigation test utility to debug the add account flow
import { router } from 'expo-router';

export const testNavigationFlow = () => {
  console.log('=== Navigation Test ===');
  console.log('Current route:', router);
  
  // Log navigation methods available
  console.log('Available router methods:', Object.keys(router));
  
  // Test different navigation approaches
  const testApproaches = {
    approach1: () => {
      console.log('Testing approach 1: router.replace("/(tabs)")');
      try {
        router.replace('/(tabs)');
      } catch (e) {
        console.error('Approach 1 failed:', e);
      }
    },
    
    approach2: () => {
      console.log('Testing approach 2: router.push("/(tabs)")');
      try {
        router.push('/(tabs)');
      } catch (e) {
        console.error('Approach 2 failed:', e);
      }
    },
    
    approach3: () => {
      console.log('Testing approach 3: router.navigate("/(tabs)")');
      try {
        if ('navigate' in router) {
          (router as any).navigate('/(tabs)');
        } else {
          console.log('navigate method not available');
        }
      } catch (e) {
        console.error('Approach 3 failed:', e);
      }
    },
    
    approach4: () => {
      console.log('Testing approach 4: Multiple backs then push');
      try {
        // First try to go back to clear the stack
        while ((router as any).canGoBack?.()) {
          router.back();
        }
        router.push('/(tabs)');
      } catch (e) {
        console.error('Approach 4 failed:', e);
      }
    },
    
    approach5: () => {
      console.log('Testing approach 5: dismissAll then replace');
      try {
        if ('dismissAll' in router && typeof (router as any).dismissAll === 'function') {
          (router as any).dismissAll();
        } else {
          console.log('dismissAll method not available');
        }
        router.replace('/(tabs)');
      } catch (e) {
        console.error('Approach 5 failed:', e);
      }
    },
    
    approach6: () => {
      console.log('Testing approach 6: Reset to root then navigate');
      try {
        router.replace('/');
        setTimeout(() => {
          router.replace('/(tabs)');
        }, 100);
      } catch (e) {
        console.error('Approach 6 failed:', e);
      }
    },
    
    approach8: () => {
      console.log('Testing approach 8: Using Link.href pattern');
      try {
        // This mimics what Link components do internally
        router.replace({ pathname: '/(tabs)' } as any);
      } catch (e) {
        console.error('Approach 8 failed:', e);
      }
    },
    
    approach9: () => {
      console.log('Testing approach 9: Back to root and then to tabs with delay');
      try {
        // Go to root first, then navigate
        router.replace('/');
        setTimeout(() => {
          router.push('/(tabs)');
        }, 200);
      } catch (e) {
        console.error('Approach 9 failed:', e);
      }
    },

    approach10: () => {
      console.log('Testing approach 10: Using router.dismiss + replace');
      try {
        // Try to dismiss the current modal first
        if ((router as any).canDismiss?.()) {
          (router as any).dismiss();
        }
        // Then replace with tabs
        setTimeout(() => {
          router.replace('/(tabs)');
        }, 100);
      } catch (e) {
        console.error('Approach 10 failed:', e);
      }
    }
  };
  
  return testApproaches;
};

export const logNavigationState = () => {
  console.log('=== Navigation State ===');
  console.log('Router object keys:', Object.keys(router));
  
  // Try to access route state if available
  try {
    const routerObj = router as any;
    console.log('Current pathname:', routerObj.pathname);
    console.log('Can go back:', routerObj.canGoBack?.());
    console.log('Can dismiss:', routerObj.canDismiss?.());
    console.log('Can go forward:', routerObj.canGoForward?.());
    
    // Log all available methods
    const methods = Object.keys(routerObj).filter(key => typeof routerObj[key] === 'function');
    console.log('Available methods:', methods);
  } catch (e) {
    console.log('Could not access navigation state:', e);
  }
};
