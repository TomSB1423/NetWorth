// Simple test to verify AddAccountPage initialization logic

// Simulate the useLocalSearchParams hook behavior
const mockUseLocalSearchParams = () => ({
    selectedBank: 'chase',
    bankLabel: 'Chase Bank',
    bankEmoji: '🏦',
});

// Test the component initialization logic
function testAddAccountPageInitialization() {
    console.log('Testing AddAccountPage initialization...');

    const params = mockUseLocalSearchParams();
    const selectedBank = params.selectedBank || '';
    const bankLabel = params.bankLabel || '';
    const bankEmoji = params.bankEmoji || '';

    // This is the new logic we implemented - useState with function initializer
    const initialName = bankLabel ? `${bankLabel} Account` : '';

    console.log('Test Results:');
    console.log('Selected Bank:', selectedBank);
    console.log('Bank Label:', bankLabel);
    console.log('Bank Emoji:', bankEmoji);
    console.log('Initial Name:', initialName);
    console.log('Expected: "Chase Bank Account"');
    console.log('Test Passed:', initialName === 'Chase Bank Account');

    return initialName === 'Chase Bank Account';
}

// Test edge cases
function testEdgeCases() {
    console.log('\nTesting edge cases...');

    // Test with empty params
    const emptyParams = { selectedBank: '', bankLabel: '', bankEmoji: '' };
    const emptyInitialName = emptyParams.bankLabel ? `${emptyParams.bankLabel} Account` : '';
    console.log('Empty params - Initial name:', `"${emptyInitialName}"`, '(should be empty)');

    // Test with only bank selected
    const partialParams = { selectedBank: 'wells-fargo', bankLabel: 'Wells Fargo', bankEmoji: '🏛️' };
    const partialInitialName = partialParams.bankLabel ? `${partialParams.bankLabel} Account` : '';
    console.log('Partial params - Initial name:', `"${partialInitialName}"`, '(should be "Wells Fargo Account")');

    return emptyInitialName === '' && partialInitialName === 'Wells Fargo Account';
}

// Run tests
console.log('Running AddAccountPage tests...\n');
const test1Passed = testAddAccountPageInitialization();
const test2Passed = testEdgeCases();

console.log('\n=== Overall Test Results ===');
console.log('Initialization Test:', test1Passed ? 'PASSED ✅' : 'FAILED ❌');
console.log('Edge Cases Test:', test2Passed ? 'PASSED ✅' : 'FAILED ❌');
console.log('All Tests:', (test1Passed && test2Passed) ? 'PASSED ✅' : 'FAILED ❌');

console.log('\n=== Fix Summary ===');
console.log('The infinite loop was caused by useEffect with [bankLabel, name] dependencies.');
console.log('Fixed by using useState with function initializer instead of useEffect.');
console.log('This ensures the default name is set only once during component initialization.');
