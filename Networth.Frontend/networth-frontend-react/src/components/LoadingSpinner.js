/**
 * Loading Spinner Component
 */

import React from 'react';
import PropTypes from 'prop-types';

const LoadingSpinner = ({ size = 'medium', text = 'Loading...' }) => {
    const sizeClasses = {
        small: 'w-4 h-4',
        medium: 'w-8 h-8',
        large: 'w-12 h-12'
    };

    return (
        <div className="flex flex-col items-center justify-center p-8">
            <div className={`${sizeClasses[size]} animate-spin rounded-full border-4 border-gray-200 border-t-indigo-600`}></div>
            {text && <p className="mt-2 text-gray-600 text-sm">{text}</p>}
        </div>
    );
};

LoadingSpinner.propTypes = {
    size: PropTypes.oneOf(['small', 'medium', 'large']),
    text: PropTypes.string,
};

export default LoadingSpinner;
