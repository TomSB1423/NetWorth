import { generateSecureId, isValidAccountName, sanitizeString } from '../utils/security';
import { formatCurrency, formatPercentage, validateAccount, validateTransaction } from '../utils/validation';

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
        expect(id).toMatch(/^[a-zA-Z0-9]+$/);
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

  describe('Validation Utils', () => {
    describe('validateAccount', () => {
      const validAccount = {
        id: '123',
        name: 'Test Account',
        type: 'checking' as const,
        transactions: []
      };

      it('validates correct account structure', () => {
        const result = validateAccount(validAccount);
        expect(result.isValid).toBe(true);
        expect(result.errors).toHaveLength(0);
      });

      it('catches missing required fields', () => {
        const invalidAccount = {
          id: '',
          name: '',
          type: 'checking' as const,
          transactions: []
        };

        const result = validateAccount(invalidAccount);
        expect(result.isValid).toBe(false);
        expect(result.errors.length).toBeGreaterThan(0);
      });

      it('validates account type', () => {
        const invalidAccount = {
          ...validAccount,
          type: 'invalid' as any
        };

        const result = validateAccount(invalidAccount);
        expect(result.isValid).toBe(false);
        expect(result.errors.some(error => error.includes('type'))).toBe(true);
      });
    });

    describe('validateTransaction', () => {
      const validTransaction = {
        id: '123',
        date: '2024-01-01',
        amount: 100,
        description: 'Test transaction',
        category: 'food' as const
      };

      it('validates correct transaction structure', () => {
        const result = validateTransaction(validTransaction);
        expect(result.isValid).toBe(true);
        expect(result.errors).toHaveLength(0);
      });

      it('catches invalid amounts', () => {
        const invalidTransaction = {
          ...validTransaction,
          amount: NaN
        };

        const result = validateTransaction(invalidTransaction);
        expect(result.isValid).toBe(false);
      });

      it('validates date format', () => {
        const invalidTransaction = {
          ...validTransaction,
          date: 'invalid-date'
        };

        const result = validateTransaction(invalidTransaction);
        expect(result.isValid).toBe(false);
      });
    });

    describe('formatCurrency', () => {
      it('formats positive amounts correctly', () => {
        expect(formatCurrency(1234.56)).toBe('$1,234.56');
        expect(formatCurrency(0)).toBe('$0.00');
        expect(formatCurrency(1000000)).toBe('$1,000,000.00');
      });

      it('formats negative amounts correctly', () => {
        expect(formatCurrency(-1234.56)).toBe('-$1,234.56');
        expect(formatCurrency(-100)).toBe('-$100.00');
      });

      it('handles edge cases', () => {
        expect(formatCurrency(0.01)).toBe('$0.01');
        expect(formatCurrency(0.999)).toBe('$1.00'); // rounds up
        expect(formatCurrency(NaN)).toBe('$0.00');
      });
    });

    describe('formatPercentage', () => {
      it('formats percentages correctly', () => {
        expect(formatPercentage(0.25)).toBe('25.0%');
        expect(formatPercentage(0.1234)).toBe('12.3%');
        expect(formatPercentage(1.0)).toBe('100.0%');
      });

      it('handles negative percentages', () => {
        expect(formatPercentage(-0.1)).toBe('-10.0%');
      });

      it('handles edge cases', () => {
        expect(formatPercentage(0)).toBe('0.0%');
        expect(formatPercentage(NaN)).toBe('0.0%');
        expect(formatPercentage(Infinity)).toBe('0.0%');
      });
    });
  });

  describe('Error Handling', () => {
    it('handles malformed input gracefully', () => {
      // Test that utility functions don't crash with bad input
      expect(() => sanitizeString(123 as any)).not.toThrow();
      expect(() => isValidAccountName(123 as any)).not.toThrow();
      expect(() => formatCurrency('invalid' as any)).not.toThrow();
      expect(() => formatPercentage('invalid' as any)).not.toThrow();
    });

    it('provides meaningful error messages', () => {
      const result = validateAccount({} as any);
      expect(result.errors.length).toBeGreaterThan(0);
      expect(result.errors[0]).toContain('required');
    });
  });
});
