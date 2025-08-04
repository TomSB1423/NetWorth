import React from 'react';
import InstitutionCard from './InstitutionCard';
import './InstitutionList.css';

/**
 * InstitutionList component to display a list of institutions
 */
function InstitutionList({
    institutions,
    onInstitutionClick,
    title = 'Available Institutions',
    emptyMessage = 'No institutions found.'
}) {
    if (institutions.length === 0) {
        return (
            <div className="institution-list-empty">
                <p>{emptyMessage}</p>
            </div>
        );
    }

    return (
        <div className="institution-list-container">
            <h2 className="institution-list-title">
                {title} ({institutions.length})
            </h2>
            <div className="institution-list-grid">
                {institutions.map((institution) => (
                    <InstitutionCard
                        key={institution.id}
                        institution={institution}
                        onClick={onInstitutionClick ? () => onInstitutionClick(institution) : undefined}
                    />
                ))}
            </div>
        </div>
    );
}

export default InstitutionList;
