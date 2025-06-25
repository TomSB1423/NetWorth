import { useState } from "react";
import "./App.css";
import { getInstitutions } from "../services/institutionsService";

function App() {
  const [institutions, setInstitutions] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchInstitutions = async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await getInstitutions();
      setInstitutions(data.value || []);
    } catch (err) {
      setError('Failed to fetch institutions. Please try again.');
      console.error('Error:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>Financial Institutions</h1>

        <button
          onClick={fetchInstitutions}
          disabled={loading}
        >
          {loading ? 'Loading...' : 'Fetch Institutions'}
        </button>

        {error && (
          <div style={{
            color: '#ff6b6b',
            marginBottom: '20px',
            padding: '10px',
            border: '1px solid #ff6b6b',
            borderRadius: '4px',
            backgroundColor: 'rgba(255, 107, 107, 0.1)'
          }}>
            {error}
          </div>
        )}

        {institutions.length > 0 && (
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Transaction Days</th>
                <th>Max Access Days</th>
                <th>Logo</th>
              </tr>
            </thead>
            <tbody>
              {institutions.map((institution) => (
                <tr key={institution.id}>
                  <td>{institution.id}</td>
                  <td>{institution.name}</td>
                  <td>{institution.transactionTotalDays}</td>
                  <td>{institution.maxAccessValidForDays}</td>
                  <td>
                    {institution.logoUrl && (
                      <img
                        src={institution.logoUrl}
                        alt={`${institution.name} logo`}
                        style={{ width: '32px', height: '32px', objectFit: 'contain' }}
                      />
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        {institutions.length === 0 && !loading && !error && (
          <p>Click the button to fetch institutions data.</p>
        )}
      </header>
    </div>
  );
}

export default App;
