import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { institutionService, accountService } from '../services/api';
import { LoadingSpinner } from '../components';
import { PageHeader, PageContainer } from '../components/layout';
import { Search, Building2, CheckCircle, ExternalLink } from 'lucide-react';
import './LinkInstitutionPage.css';

/**
 * LinkInstitutionPage - Page for browsing and linking financial institutions
 */
function LinkInstitutionPage() {
    const navigate = useNavigate();
    const [institutions, setInstitutions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [linkingInstitution, setLinkingInstitution] = useState(null);
    const [authUrl, setAuthUrl] = useState(null);

    useEffect(() => {
        loadInstitutions();
    }, []);

    const loadInstitutions = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await institutionService.getInstitutions();
            setInstitutions(data);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleLinkInstitution = async (institution) => {
        try {
            setLinkingInstitution(institution.id);
            setError(null);

            const response = await accountService.linkAccount(institution.id);

            // Open authorization link in new window
            setAuthUrl(response.authorizationLink);

            // Open in new tab
            window.open(response.authorizationLink, '_blank');

        } catch (err) {
            setError(`Failed to link ${institution.name}: ${err.message}`);
            setLinkingInstitution(null);
        }
    };

    const handleAuthComplete = async () => {
        if (linkingInstitution) {
            // Sync the institution to fetch account data
            try {
                await institutionService.syncInstitution(linkingInstitution);
                setLinkingInstitution(null);
                setAuthUrl(null);
                navigate('/accounts/list');
            } catch (err) {
                setError(`Failed to sync institution: ${err.message}`);
            }
        }
    };

    const filteredInstitutions = institutions.filter(inst =>
        inst.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (loading) {
        return (
            <PageContainer>
                <LoadingSpinner text="Loading institutions..." size="large" />
            </PageContainer>
        );
    }

    return (
        <PageContainer>
            <PageHeader
                title="Link Bank Account"
                subtitle="Choose your bank to securely connect your accounts"
                showBack={true}
                backTo="/accounts/list"
            />

            {error && (
                <div className="error-banner">
                    <p>{error}</p>
                    <button onClick={() => setError(null)}>Dismiss</button>
                </div>
            )}

            {authUrl && (
                <div className="auth-banner">
                    <div className="auth-banner-content">
                        <CheckCircle size={24} />
                        <div>
                            <h3>Authorization Link Created</h3>
                            <p>Please complete the authorization in the opened window, then click "Continue" below.</p>
                        </div>
                    </div>
                    <div className="auth-banner-actions">
                        <a
                            href={authUrl}
                            target="_blank"
                            rel="noopener noreferrer"
                            className="btn-secondary"
                        >
                            <ExternalLink size={16} />
                            Re-open Authorization
                        </a>
                        <button
                            onClick={handleAuthComplete}
                            className="btn-primary"
                        >
                            Continue
                        </button>
                    </div>
                </div>
            )}

            <div className="search-box">
                <Search size={20} />
                <input
                    type="text"
                    placeholder="Search institutions..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                />
            </div>

            <div className="institutions-grid">
                {filteredInstitutions.map((institution) => (
                    <div
                        key={institution.id}
                        className={`institution-card ${linkingInstitution === institution.id ? 'linking' : ''}`}
                    >
                        <div className="institution-logo">
                            {institution.logoUrl ? (
                                <img src={institution.logoUrl} alt={institution.name} />
                            ) : (
                                <Building2 size={40} />
                            )}
                        </div>
                        <h3>{institution.name}</h3>
                        <button
                            onClick={() => handleLinkInstitution(institution)}
                            disabled={linkingInstitution !== null}
                            className="btn-link"
                        >
                            {linkingInstitution === institution.id ? (
                                <>
                                    <LoadingSpinner size="small" />
                                    Linking...
                                </>
                            ) : (
                                'Link Account'
                            )}
                        </button>
                    </div>
                ))}
            </div>

            {filteredInstitutions.length === 0 && (
                <div className="empty-state">
                    <Building2 size={64} />
                    <h3>No institutions found</h3>
                    <p>Try adjusting your search term</p>
                </div>
            )}
        </PageContainer>
    );
}

export default LinkInstitutionPage;
