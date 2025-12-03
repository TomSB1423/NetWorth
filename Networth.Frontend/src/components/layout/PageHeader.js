import React from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft, RefreshCw } from 'lucide-react';
import PropTypes from 'prop-types';
import './PageHeader.css';

/**
 * PageHeader - Reusable header component with back button and actions
 */
const PageHeader = ({ 
    title, 
    subtitle, 
    showBack = false, 
    backTo = '/',
    onRefresh = null,
    refreshing = false,
    actions = null,
    children
}) => {
    const navigate = useNavigate();

    const handleBack = () => {
        if (backTo) {
            navigate(backTo);
        } else {
            navigate(-1);
        }
    };

    return (
        <div className="page-header-container">
            <div className="page-header-main">
                <div className="page-header-left">
                    {showBack && (
                        <button onClick={handleBack} className="btn-back" aria-label="Go back">
                            <ArrowLeft size={20} />
                        </button>
                    )}
                    <div className="page-header-text">
                        <h1 className="page-title">{title}</h1>
                        {subtitle && <p className="page-subtitle">{subtitle}</p>}
                    </div>
                </div>
                <div className="page-header-right">
                    {onRefresh && (
                        <button 
                            onClick={onRefresh} 
                            disabled={refreshing}
                            className="btn-refresh"
                            aria-label="Refresh"
                        >
                            <RefreshCw size={16} className={refreshing ? 'spinning' : ''} />
                            Refresh
                        </button>
                    )}
                    {actions}
                </div>
            </div>
            {children && <div className="page-header-extra">{children}</div>}
        </div>
    );
};

PageHeader.propTypes = {
    title: PropTypes.string.isRequired,
    subtitle: PropTypes.string,
    showBack: PropTypes.bool,
    backTo: PropTypes.string,
    onRefresh: PropTypes.func,
    refreshing: PropTypes.bool,
    actions: PropTypes.node,
    children: PropTypes.node,
};

export default PageHeader;
