/**
 * Loading Spinner Component
 */

import React from 'react';
import PropTypes from 'prop-types';
import './LoadingSpinner.css';

const LoadingSpinner = ({ size = 'medium', text = 'Loading...' }) => {
    return (
        <div className="loading-spinner-container">
            <div className={`loading-spinner loading-spinner-${size}`}></div>
            {text && <p className="loading-spinner-text">{text}</p>}
        </div>
    );
};

LoadingSpinner.propTypes = {
    size: PropTypes.oneOf(['small', 'medium', 'large']),
    text: PropTypes.string,
};

export default LoadingSpinner;
