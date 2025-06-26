import React, { useState } from 'react';
import { Home, CreditCard, Settings, BarChart3, TrendingUp, DollarSign, Calendar, Wallet } from 'lucide-react';

const NetWorthAppWireframe = () => {
  const [activeScreen, setActiveScreen] = useState('dashboard');

  const screens = [
    { id: 'dashboard', name: 'Dashboard', icon: Home },
    { id: 'accounts', name: 'Accounts', icon: CreditCard },
    { id: 'insights', name: 'Insights', icon: BarChart3 },
    { id: 'settings', name: 'Settings', icon: Settings }
  ];

  const renderDashboard = () => (
    <div className="h-full bg-gray-900 text-white p-4 overflow-y-auto">
      {/* Header */}
      <div className="text-center mb-8 pt-4">
        <div className="text-4xl font-light mb-2">$125,450</div>
        <div className="w-full h-1 bg-gray-700 rounded mb-4"></div>
      </div>

      {/* Time Period Selector */}
      <div className="flex justify-center gap-4 mb-6">
        {['ALL', '1y', '6m', '3m', '1w'].map((period) => (
          <button 
            key={period}
            className={`px-3 py-1 rounded-full text-sm ${
              period === '1w' ? 'border border-white' : 'text-gray-400'
            }`}
          >
            {period}
          </button>
        ))}
      </div>

      {/* Metrics Grid */}
      <div className="grid grid-cols-2 gap-4 mb-6">
        <div className="bg-gray-800 p-4 rounded-lg">
          <div className="flex items-center gap-2 mb-2">
            <DollarSign className="w-4 h-4 text-yellow-500" />
            <span className="text-xs text-gray-400">MONTHLY INCOME</span>
          </div>
          <div className="text-2xl font-semibold">$6,500</div>
          <div className="text-xs text-gray-400">Last 30 days</div>
        </div>

        <div className="bg-gray-800 p-4 rounded-lg">
          <div className="flex items-center gap-2 mb-2">
            <Wallet className="w-4 h-4 text-red-400" />
            <span className="text-xs text-gray-400">MONTHLY EXPENSES</span>
          </div>
          <div className="text-2xl font-semibold">$4,800</div>
          <div className="text-xs text-gray-400">Last 30 days</div>
        </div>

        <div className="bg-gray-800 p-4 rounded-lg">
          <div className="flex items-center gap-2 mb-2">
            <TrendingUp className="w-4 h-4 text-green-400" />
            <span className="text-xs text-gray-400">SAVINGS RATE</span>
          </div>
          <div className="text-2xl font-semibold">26.2%</div>
          <div className="text-xs text-gray-400">Income - Expenses</div>
          <div className="text-xs text-green-400">↗ 5.2%</div>
        </div>

        <div className="bg-gray-800 p-4 rounded-lg">
          <div className="flex items-center gap-2 mb-2">
            <BarChart3 className="w-4 h-4 text-blue-400" />
            <span className="text-xs text-gray-400">AVG TRANSACTION</span>
          </div>
          <div className="text-2xl font-semibold">$127</div>
          <div className="text-xs text-gray-400">All time</div>
        </div>

        <div className="bg-gray-800 p-4 rounded-lg">
          <div className="flex items-center gap-2 mb-2">
            <CreditCard className="w-4 h-4 text-purple-400" />
            <span className="text-xs text-gray-400">TOTAL BALANCE</span>
          </div>
          <div className="text-2xl font-semibold">$125,450</div>
          <div className="text-xs text-gray-400">Assets - Liabilities</div>
          <div className="text-xs text-green-400">↗ 12.5%</div>
        </div>

        <div className="bg-gray-800 p-4 rounded-lg">
          <div className="flex items-center gap-2 mb-2">
            <DollarSign className="w-4 h-4 text-orange-400" />
            <span className="text-xs text-gray-400">LARGEST EXPENSE</span>
          </div>
          <div className="text-2xl font-semibold">$1,850</div>
          <div className="text-xs text-gray-400">Last 30 days</div>
        </div>
      </div>

      {/* Cash Flow Section */}
      <div className="bg-gray-800 p-4 rounded-lg">
        <h3 className="text-lg font-semibold mb-4">Monthly Cash Flow</h3>
        <div className="h-32 bg-gray-700 rounded flex items-center justify-center">
          <span className="text-gray-500">Chart Placeholder</span>
        </div>
      </div>
    </div>
  );

  const renderAccounts = () => (
    <div className="h-full bg-gray-50 p-4 overflow-y-auto">
      <div className="bg-white rounded-lg p-4 mb-4">
        <h2 className="text-xl font-semibold mb-4">Connected Accounts</h2>
        
        {/* Account List */}
        <div className="space-y-3">
          <div className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-blue-500 rounded-full flex items-center justify-center">
                <span className="text-white font-semibold">C</span>
              </div>
              <div>
                <div className="font-medium">Chase Checking</div>
                <div className="text-sm text-gray-500">****1234</div>
              </div>
            </div>
            <div className="text-right">
              <div className="font-semibold">$8,750</div>
              <div className="text-sm text-green-600">+2.3%</div>
            </div>
          </div>

          <div className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-green-500 rounded-full flex items-center justify-center">
                <span className="text-white font-semibold">S</span>
              </div>
              <div>
                <div className="font-medium">Savings Account</div>
                <div className="text-sm text-gray-500">****5678</div>
              </div>
            </div>
            <div className="text-right">
              <div className="font-semibold">$42,300</div>
              <div className="text-sm text-green-600">+1.8%</div>
            </div>
          </div>

          <div className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-purple-500 rounded-full flex items-center justify-center">
                <span className="text-white font-semibold">I</span>
              </div>
              <div>
                <div className="font-medium">Investment Account</div>
                <div className="text-sm text-gray-500">****9012</div>
              </div>
            </div>
            <div className="text-right">
              <div className="font-semibold">$84,200</div>
              <div className="text-sm text-green-600">+15.2%</div>
            </div>
          </div>

          <div className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-red-500 rounded-full flex items-center justify-center">
                <span className="text-white font-semibold">L</span>
              </div>
              <div>
                <div className="font-medium">Credit Card</div>
                <div className="text-sm text-gray-500">****3456</div>
              </div>
            </div>
            <div className="text-right">
              <div className="font-semibold text-red-600">-$9,800</div>
              <div className="text-sm text-red-600">68% util.</div>
            </div>
          </div>
        </div>

        <button className="w-full mt-4 p-3 border-2 border-dashed border-gray-300 rounded-lg text-center text-gray-500 hover:border-blue-400 hover:text-blue-600">
          + Add Account
        </button>
      </div>
    </div>
  );

  const renderInsights = () => (
    <div className="h-full bg-gray-50 p-4 overflow-y-auto">
      <div className="space-y-4">
        {/* Key Metrics */}
        <div className="bg-white rounded-lg p-4">
          <h2 className="text-xl font-semibold mb-4">Financial Health</h2>
          <div className="grid grid-cols-2 gap-4">
            <div className="text-center p-3 bg-green-50 rounded-lg">
              <div className="text-2xl font-bold text-green-700">26%</div>
              <div className="text-sm text-green-600">Savings Rate</div>
            </div>
            <div className="text-center p-3 bg-blue-50 rounded-lg">
              <div className="text-2xl font-bold text-blue-700">4.2</div>
              <div className="text-sm text-blue-600">Emergency Fund (months)</div>
            </div>
          </div>
        </div>

        {/* Goals */}
        <div className="bg-white rounded-lg p-4">
          <h2 className="text-xl font-semibold mb-4">Goals Progress</h2>
          <div className="space-y-3">
            <div>
              <div className="flex justify-between mb-1">
                <span className="text-sm font-medium">Emergency Fund</span>
                <span className="text-sm text-gray-500">73%</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-blue-600 h-2 rounded-full" style={{width: '73%'}}></div>
              </div>
            </div>
            <div>
              <div className="flex justify-between mb-1">
                <span className="text-sm font-medium">Debt Payoff</span>
                <span className="text-sm text-gray-500">41%</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-green-600 h-2 rounded-full" style={{width: '41%'}}></div>
              </div>
            </div>
          </div>
        </div>

        {/* Spending Categories */}
        <div className="bg-white rounded-lg p-4">
          <h2 className="text-xl font-semibold mb-4">Top Spending Categories</h2>
          <div className="space-y-2">
            {[
              { name: 'Housing', amount: '$1,850', percent: '38%' },
              { name: 'Food & Dining', amount: '$720', percent: '15%' },
              { name: 'Transportation', amount: '$480', percent: '10%' },
              { name: 'Utilities', amount: '$320', percent: '7%' }
            ].map((category, index) => (
              <div key={index} className="flex justify-between items-center">
                <span className="text-sm">{category.name}</span>
                <div className="text-right">
                  <span className="font-medium">{category.amount}</span>
                  <span className="text-sm text-gray-500 ml-2">{category.percent}</span>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Alerts */}
        <div className="bg-white rounded-lg p-4">
          <h2 className="text-xl font-semibold mb-4">Smart Alerts</h2>
          <div className="space-y-3">
            <div className="p-3 bg-yellow-50 border-l-4 border-yellow-400 rounded">
              <div className="text-sm font-medium text-yellow-800">Unusual Spending</div>
              <div className="text-xs text-yellow-700">You spent 23% more on dining this month</div>
            </div>
            <div className="p-3 bg-green-50 border-l-4 border-green-400 rounded">
              <div className="text-sm font-medium text-green-800">Investment Opportunity</div>
              <div className="text-xs text-green-700">$2,400 in low-yield savings could be invested</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );

  const renderSettings = () => (
    <div className="h-full bg-gray-50 p-4 overflow-y-auto">
      <div className="bg-white rounded-lg p-4">
        <h2 className="text-xl font-semibold mb-4">Settings</h2>
        
        <div className="space-y-4">
          <div className="border-b pb-4">
            <h3 className="font-medium mb-2">Account Management</h3>
            <div className="space-y-2">
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded">Sync Accounts</button>
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded">Connection Status</button>
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded">Data Refresh</button>
            </div>
          </div>

          <div className="border-b pb-4">
            <h3 className="font-medium mb-2">Preferences</h3>
            <div className="space-y-2">
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded">Notifications</button>
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded">Currency</button>
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded">Time Zone</button>
            </div>
          </div>

          <div className="border-b pb-4">
            <h3 className="font-medium mb-2">Security</h3>
            <div className="space-y-2">
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded">Privacy Settings</button>
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded">Two-Factor Auth</button>
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded">Export Data</button>
            </div>
          </div>

          <div>
            <h3 className="font-medium mb-2">Support</h3>
            <div className="space-y-2">
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded">Help Center</button>
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded">Contact Support</button>
              <button className="w-full text-left p-2 hover:bg-gray-50 rounded text-red-600">Sign Out</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );

  const renderContent = () => {
    switch(activeScreen) {
      case 'dashboard': return renderDashboard();
      case 'accounts': return renderAccounts();
      case 'insights': return renderInsights();
      case 'settings': return renderSettings();
      default: return renderDashboard();
    }
  };

  return (
    <div className="max-w-sm mx-auto bg-white border border-gray-300 rounded-3xl overflow-hidden shadow-2xl">
      {/* Status Bar */}
      <div className="bg-black text-white px-6 py-2 flex justify-between items-center text-sm">
        <span>9:41</span>
        <div className="flex items-center gap-1">
          <div className="flex gap-1">
            <div className="w-1 h-1 bg-white rounded-full"></div>
            <div className="w-1 h-1 bg-white rounded-full"></div>
            <div className="w-1 h-1 bg-white rounded-full"></div>
          </div>
          <div className="flex gap-1 ml-2">
            <div className="w-4 h-2 border border-white rounded-sm">
              <div className="w-3 h-1 bg-white rounded-sm m-0.5"></div>
            </div>
          </div>
        </div>
        <div className="w-6 h-3 border border-white rounded-sm">
          <div className="w-5 h-2 bg-white rounded-sm m-0.5"></div>
        </div>
      </div>

      {/* Main Content */}
      <div className="h-[600px] relative">
        {renderContent()}
      </div>

      {/* Bottom Navigation */}
      <div className="bg-white border-t border-gray-200 px-4 py-2">
        <div className="flex justify-around">
          {screens.map((screen) => {
            const Icon = screen.icon;
            return (
              <button
                key={screen.id}
                onClick={() => setActiveScreen(screen.id)}
                className={`flex flex-col items-center py-2 px-3 rounded-lg ${
                  activeScreen === screen.id
                    ? 'text-blue-600'
                    : 'text-gray-400'
                }`}
              >
                <Icon className="w-5 h-5 mb-1" />
                <span className="text-xs">{screen.name}</span>
              </button>
            );
          })}
        </div>
      </div>

      {/* Home Indicator */}
      <div className="bg-white pb-2 flex justify-center">
        <div className="w-32 h-1 bg-gray-300 rounded-full"></div>
      </div>
    </div>
  );
};

export default NetWorthAppWireframe;