// __tests__/file-structure.test.ts
// Test to verify file structure organization

import { existsSync } from 'fs';
import path from 'path';

describe('File Structure Organization', () => {
  it('should have consolidated contexts directory', () => {
    expect(existsSync(path.join(__dirname, '../contexts'))).toBe(true);
    expect(existsSync(path.join(__dirname, '../context'))).toBe(false);
  });

  it('should have essential directories', () => {
    const essentialDirs = [
      'app',
      'components', 
      'contexts',
      'store',
      'services',
      'utils',
      'constants',
      'hooks',
      'types',
      'assets',
      'docs'
    ];

    essentialDirs.forEach(dir => {
      expect(existsSync(path.join(__dirname, '..', dir))).toBe(true);
    });
  });

  it('should have documentation for file structure', () => {
    expect(existsSync(path.join(__dirname, '../docs/FILE_STRUCTURE_REVIEW.md'))).toBe(true);
  });
});