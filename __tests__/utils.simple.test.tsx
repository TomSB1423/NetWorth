import { generateSecureId, isValidAccountName, sanitizeString } from '../utils/security';

describe('Utility Functions Tests', () => {
  describe('Security Utils', () => {
    describe('generateSecureId', () => {
      it('generates unique IDs', () => {
        const id1 = generateSecureId();
        const id2 = generateSecureId();
        
        expect(id1).toBeTruthy();
        expect(id2).toBeTruthy();
        expect(id1).not.toBe(id2);
        expect(typeof id1).toBe('string');
        expect(id1.length).toBeGreaterThan(0);
      });

      it('generates IDs of consistent format', () => {
        const id = generateSecureId();
        
        // Should be alphanumeric
        expect(id).toMatch(/^[a-zA-Z0-9_-]+$/);
        expect(id.length).toBeGreaterThanOrEqual(8);
      });
    });

    describe('sanitizeString', () => {
      it('removes dangerous characters', () => {
        const dangerous = '<script>alert("hack")</script>';
        const sanitized = sanitizeString(dangerous);
        
        expect(sanitized).not.toContain('<script>');
        expect(sanitized).not.toContain('</script>');
      });

      it('preserves safe characters', () => {
        const safe = 'My Bank Account 123';
        const sanitized = sanitizeString(safe);
        
        expect(sanitized).toBe(safe);
      });

      it('handles empty and null inputs', () => {
        expect(sanitizeString('')).toBe('');
        expect(sanitizeString(null as any)).toBe('');
        expect(sanitizeString(undefined as any)).toBe('');
      });

      it('trims whitespace', () => {
        const input = '  account name  ';
        const sanitized = sanitizeString(input);
        
        expect(sanitized).toBe('account name');
      });
    });

    describe('isValidAccountName', () => {
      it('accepts valid account names', () => {
        expect(isValidAccountName('Chase Checking')).toBe(true);
        expect(isValidAccountName('Savings Account')).toBe(true);
        expect(isValidAccountName('My Investment 401k')).toBe(true);
      });

      it('rejects invalid account names', () => {
        expect(isValidAccountName('')).toBe(false);
        expect(isValidAccountName('   ')).toBe(false);
        expect(isValidAccountName('<script>')).toBe(false);
        expect(isValidAccountName('a')).toBe(false); // too short
      });

      it('handles edge cases', () => {
        expect(isValidAccountName(null as any)).toBe(false);
        expect(isValidAccountName(undefined as any)).toBe(false);
        expect(isValidAccountName('A'.repeat(101))).toBe(false); // too long
      });
    });
  });

  describe('Error Handling', () => {
    it('handles malformed input gracefully', () => {
      // Test that utility functions don't crash with bad input
      expect(() => sanitizeString(123 as any)).not.toThrow();
      expect(() => isValidAccountName(123 as any)).not.toThrow();
    });
  });
});
