import React from 'react';
import './ErrorMessage.css';

/**
 * ErrorMessage component to display error messages
 */
function ErrorMessage({
    title = 'Error',
    message,
    onRetry,
    retryButtonText = 'Try Again'
}) {
    return (
        <div className="error-container">
            <h3 className="error-title">{title}</h3>
            <p className="error-message">{message}</p>
            {onRetry && (
                <button
                    className="error-retry-button"
                    onClick={onRetry}
                    type="button"
                >
                    {retryButtonText}
                </button>
            )}
        </div>
    );
}

export default ErrorMessage;
