// utils/security.ts
// Security utilities for input validation and sanitization

/**
 * Sanitizes user input to prevent XSS and other injection attacks
 */
export function sanitizeString(input: string): string {
  if (typeof input !== 'string') {
    return '';
  }
  
  // Remove potentially dangerous characters
  return input
    .replace(/[<>\"']/g, '') // Remove HTML/script tags
    .replace(/javascript:/gi, '') // Remove javascript: protocol
    .replace(/data:/gi, '') // Remove data: protocol
    .trim();
}

/**
 * Validates that a string contains only alphanumeric characters, spaces, and common punctuation
 */
export function isValidAccountName(name: string): boolean {
  if (!name || typeof name !== 'string') {
    return false;
  }
  
  // Allow letters, numbers, spaces, hyphens, apostrophes, and periods
  const validPattern = /^[a-zA-Z0-9\s\-'\.]+$/;
  return validPattern.test(name) && name.length >= 1 && name.length <= 100;
}

/**
 * Validates that a number is within a safe range
 */
export function isValidAmount(amount: number): boolean {
  if (typeof amount !== 'number' || isNaN(amount) || !isFinite(amount)) {
    return false;
  }
  
  // Prevent extremely large numbers that could cause overflow
  return Math.abs(amount) <= Number.MAX_SAFE_INTEGER / 100;
}

/**
 * Validates email format (if needed for future features)
 */
export function isValidEmail(email: string): boolean {
  if (!email || typeof email !== 'string') {
    return false;
  }
  
  const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailPattern.test(email) && email.length <= 254;
}

/**
 * Validates date format and ensures it's not in the future (for transactions)
 */
export function isValidTransactionDate(dateString: string): boolean {
  if (!dateString || typeof dateString !== 'string') {
    return false;
  }
  
  const date = new Date(dateString);
  const now = new Date();
  
  // Check if date is valid and not in the future
  return !isNaN(date.getTime()) && date <= now;
}

/**
 * Rate limiting utility for preventing spam
 */
class RateLimiter {
  private attempts: Map<string, number[]> = new Map();
  
  /**
   * Check if an action is allowed based on rate limiting
   * @param key - Unique identifier for the action/user
   * @param maxAttempts - Maximum number of attempts allowed
   * @param windowMs - Time window in milliseconds
   */
  isAllowed(key: string, maxAttempts: number = 5, windowMs: number = 60000): boolean {
    const now = Date.now();
    const attempts = this.attempts.get(key) || [];
    
    // Remove old attempts outside the time window
    const recentAttempts = attempts.filter(timestamp => now - timestamp < windowMs);
    
    if (recentAttempts.length >= maxAttempts) {
      return false;
    }
    
    // Add current attempt
    recentAttempts.push(now);
    this.attempts.set(key, recentAttempts);
    
    return true;
  }
  
  /**
   * Clear attempts for a specific key
   */
  clearAttempts(key: string): void {
    this.attempts.delete(key);
  }
}

export const rateLimiter = new RateLimiter();

/**
 * Generates a secure random ID
 */
export function generateSecureId(): string {
  const timestamp = Date.now().toString(36);
  const randomPart = crypto.randomUUID();
  return `${timestamp}_${randomPart}`;
}

/**
 * Deep freeze an object to prevent modification
 */
export function deepFreeze<T>(obj: T): T {
  Object.getOwnPropertyNames(obj).forEach(prop => {
    const value = (obj as any)[prop];
    if (value && typeof value === 'object') {
      deepFreeze(value);
    }
  });
  
  return Object.freeze(obj);
}
