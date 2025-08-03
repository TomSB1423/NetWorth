/**
 * Custom hook for managing institutions data
 */

import { useState, useEffect, useCallback } from 'react';
import { getInstitutionsService } from '../services';

export const useInstitutions = () => {
    const [institutions, setInstitutions] = useState([]);
    const [accounts, setAccounts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [syncing, setSyncing] = useState(new Set());

    const institutionsService = getInstitutionsService();

    // Load initial data
    const loadData = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);

            const [institutionsData, accountsData] = await Promise.all([
                institutionsService.getInstitutions(),
                institutionsService.getAllAccounts()
            ]);

            setInstitutions(institutionsData);
            setAccounts(accountsData);
        } catch (err) {
            console.error('Error loading institutions data:', err);
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }, [institutionsService]);

    // Sync specific institution
    const syncInstitution = useCallback(async (institutionId) => {
        try {
            setSyncing(prev => new Set([...prev, institutionId]));

            await institutionsService.syncInstitution(institutionId);

            // Reload data after sync
            const [updatedInstitutions, updatedAccounts] = await Promise.all([
                institutionsService.getInstitutions(),
                institutionsService.getAllAccounts()
            ]);

            setInstitutions(updatedInstitutions);
            setAccounts(updatedAccounts);

        } catch (err) {
            console.error('Error syncing institution:', err);
            setError(err.message);
        } finally {
            setSyncing(prev => {
                const newSet = new Set(prev);
                newSet.delete(institutionId);
                return newSet;
            });
        }
    }, [institutionsService]);

    // Connect new institution
    const connectInstitution = useCallback(async (institutionData) => {
        try {
            setLoading(true);
            const newInstitution = await institutionsService.connectInstitution(institutionData);
            setInstitutions(prev => [...prev, newInstitution]);

            // Reload accounts to include new ones
            const updatedAccounts = await institutionsService.getAllAccounts();
            setAccounts(updatedAccounts);

            return newInstitution;
        } catch (err) {
            console.error('Error connecting institution:', err);
            setError(err.message);
            throw err;
        } finally {
            setLoading(false);
        }
    }, [institutionsService]);

    // Disconnect institution
    const disconnectInstitution = useCallback(async (institutionId) => {
        try {
            await institutionsService.disconnectInstitution(institutionId);

            // Remove from local state
            setInstitutions(prev => prev.filter(inst => inst.id !== institutionId));
            setAccounts(prev => prev.filter(acc => acc.institutionId !== institutionId));

        } catch (err) {
            console.error('Error disconnecting institution:', err);
            setError(err.message);
        }
    }, [institutionsService]);

    // Load data on mount
    useEffect(() => {
        loadData();
    }, [loadData]);

    return {
        institutions,
        accounts,
        loading,
        error,
        syncing,
        syncInstitution,
        connectInstitution,
        disconnectInstitution,
        refetch: loadData,
    };
};
