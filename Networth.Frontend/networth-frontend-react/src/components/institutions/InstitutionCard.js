import React from 'react';
import './InstitutionCard.css';

/**
 * InstitutionCard component to display institution information
 */
function InstitutionCard({ institution, onClick }) {
    return (
        <div
            className={`institution-card ${onClick ? 'institution-card-clickable' : ''}`}
            onClick={onClick}
            role={onClick ? 'button' : undefined}
            tabIndex={onClick ? 0 : undefined}
            onKeyDown={onClick ? (e) => {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    onClick(institution);
                }
            } : undefined}
        >
            <div className="institution-header">
                {institution.logoUrl && (
                    <img
                        src={institution.logoUrl}
                        alt={`${institution.name} logo`}
                        className="institution-logo"
                        onError={(e) => {
                            e.target.style.display = 'none';
                        }}
                    />
                )}
                <div className="institution-info">
                    <h3 className="institution-name">{institution.name}</h3>
                    <p className="institution-id">ID: {institution.id}</p>
                </div>
            </div>

            <div className="institution-details">
                {institution.transactionTotalDays && (
                    <p className="institution-detail">
                        📊 Transaction History: {institution.transactionTotalDays} days
                    </p>
                )}
                {institution.maxAccessValidForDays && (
                    <p className="institution-detail">
                        🔐 Max Access Validity: {institution.maxAccessValidForDays} days
                    </p>
                )}
                {institution.accounts && institution.accounts.length > 0 && (
                    <p className="institution-detail">
                        🏦 Accounts: {institution.accounts.length}
                    </p>
                )}
            </div>
        </div>
    );
}

export default InstitutionCard;
