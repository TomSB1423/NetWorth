import React from 'react';
import { useInstitutions } from '../hooks';
import { LoadingSpinner, ErrorMessage, InstitutionList } from '../components';
import './InstitutionsPage.css';

/**
 * InstitutionsPage - Main page for displaying financial institutions
 */
function InstitutionsPage() {
    const { institutions, loading, error, refetch } = useInstitutions();

    const handleInstitutionClick = (institution) => {
        console.log('Institution clicked:', institution);
        // TODO: Implement navigation to institution details or account linking
    };

    if (loading) {
        return (
            <div className="institutions-page">
                <LoadingSpinner text="Loading institutions..." size="large" />
            </div>
        );
    }

    if (error) {
        return (
            <div className="institutions-page">
                <ErrorMessage
                    title="Failed to Load Institutions"
                    message={error}
                    onRetry={refetch}
                    retryButtonText="Reload Institutions"
                />
            </div>
        );
    }

    return (
        <div className="institutions-page">
            <div className="institutions-page-header">
                <h1>NetWorth - Financial Institutions</h1>
                <p className="institutions-page-subtitle">
                    Connect your bank accounts to start tracking your net worth
                </p>
            </div>

            <InstitutionList
                institutions={institutions}
                onInstitutionClick={handleInstitutionClick}
                title="Available Institutions"
                emptyMessage="No financial institutions are currently available. Please try again later."
            />
        </div>
    );
}

export default InstitutionsPage;
