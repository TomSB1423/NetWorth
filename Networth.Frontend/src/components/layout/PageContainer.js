import React from 'react';
import PropTypes from 'prop-types';
import './PageContainer.css';

/**
 * PageContainer - Wrapper for page content with consistent padding and max-width
 */
const PageContainer = ({ children, maxWidth = 'default' }) => {
    const widthClass = `page-container-${maxWidth}`;
    
    return (
        <div className={`page-container ${widthClass}`}>
            {children}
        </div>
    );
};

PageContainer.propTypes = {
    children: PropTypes.node.isRequired,
    maxWidth: PropTypes.oneOf(['small', 'default', 'large', 'full']),
};

export default PageContainer;
