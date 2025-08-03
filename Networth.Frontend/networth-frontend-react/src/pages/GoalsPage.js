/**
 * Goals Page - Financial goal tracking and management
 */

import React from 'react';
import { Plus, Target, Calendar, TrendingUp } from 'lucide-react';
import { MOCK_GOALS_DATA } from '../constants/mockData';

const GoalsPage = () => {
    const goalsData = MOCK_GOALS_DATA;

    return (
        <div>
            {/* Header */}
            <div className="flex items-center justify-between mb-8">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900 mb-2">Financial Goals</h1>
                    <p className="text-gray-600">Track your progress towards financial milestones</p>
                </div>
                <button className="flex items-center space-x-2 px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors">
                    <Plus className="w-4 h-4" />
                    <span>Add Goal</span>
                </button>
            </div>

            {/* Goals Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {goalsData.map((goal) => {
                    const progress = (goal.current / goal.target) * 100;
                    const remaining = goal.target - goal.current;
                    const daysUntilDeadline = Math.ceil((new Date(goal.deadline) - new Date()) / (1000 * 60 * 60 * 24));

                    return (
                        <div key={goal.id} className="bg-white rounded-xl p-6 border border-gray-100 hover:shadow-lg transition-shadow">
                            {/* Goal Header */}
                            <div className="flex items-start justify-between mb-4">
                                <div className="flex items-center space-x-3">
                                    <div className="w-12 h-12 rounded-lg flex items-center justify-center" style={{ backgroundColor: goal.color + '20' }}>
                                        <Target className="w-6 h-6" style={{ color: goal.color }} />
                                    </div>
                                    <div>
                                        <h3 className="font-semibold text-gray-900">{goal.name}</h3>
                                        <p className="text-sm text-gray-600">{goal.category}</p>
                                    </div>
                                </div>
                                <span className={`px-2 py-1 rounded-full text-xs font-medium ${progress >= 100 ? 'bg-green-100 text-green-700' :
                                        progress >= 75 ? 'bg-blue-100 text-blue-700' :
                                            progress >= 50 ? 'bg-yellow-100 text-yellow-700' :
                                                'bg-gray-100 text-gray-700'
                                    }`}>
                                    {progress.toFixed(0)}%
                                </span>
                            </div>

                            {/* Progress Bar */}
                            <div className="mb-4">
                                <div className="flex justify-between text-sm text-gray-600 mb-2">
                                    <span>${goal.current.toLocaleString()}</span>
                                    <span>${goal.target.toLocaleString()}</span>
                                </div>
                                <div className="w-full bg-gray-200 rounded-full h-3">
                                    <div
                                        className="h-3 rounded-full transition-all duration-300"
                                        style={{
                                            width: `${Math.min(progress, 100)}%`,
                                            backgroundColor: goal.color
                                        }}
                                    ></div>
                                </div>
                            </div>

                            {/* Goal Details */}
                            <div className="space-y-3">
                                <div className="flex items-center justify-between text-sm">
                                    <span className="text-gray-600">Remaining</span>
                                    <span className="font-medium text-gray-900">
                                        ${remaining.toLocaleString()}
                                    </span>
                                </div>

                                <div className="flex items-center justify-between text-sm">
                                    <div className="flex items-center space-x-1 text-gray-600">
                                        <Calendar className="w-4 h-4" />
                                        <span>Deadline</span>
                                    </div>
                                    <span className={`font-medium ${daysUntilDeadline < 30 ? 'text-red-600' :
                                            daysUntilDeadline < 90 ? 'text-yellow-600' :
                                                'text-gray-900'
                                        }`}>
                                        {daysUntilDeadline > 0 ? `${daysUntilDeadline} days` : 'Overdue'}
                                    </span>
                                </div>

                                {progress < 100 && (
                                    <div className="flex items-center justify-between text-sm">
                                        <div className="flex items-center space-x-1 text-gray-600">
                                            <TrendingUp className="w-4 h-4" />
                                            <span>Monthly needed</span>
                                        </div>
                                        <span className="font-medium text-gray-900">
                                            ${Math.ceil(remaining / Math.max(daysUntilDeadline / 30, 1)).toLocaleString()}
                                        </span>
                                    </div>
                                )}
                            </div>

                            {/* Action Button */}
                            <div className="mt-6 pt-4 border-t border-gray-100">
                                <button
                                    className="w-full py-2 text-sm font-medium rounded-lg transition-colors"
                                    style={{
                                        backgroundColor: goal.color + '10',
                                        color: goal.color,
                                        border: `1px solid ${goal.color}20`
                                    }}
                                >
                                    {progress >= 100 ? 'Goal Achieved! 🎉' : 'Add Contribution'}
                                </button>
                            </div>
                        </div>
                    );
                })}
            </div>

            {/* Empty State */}
            {goalsData.length === 0 && (
                <div className="text-center py-12">
                    <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <Target className="w-8 h-8 text-gray-400" />
                    </div>
                    <h3 className="text-lg font-medium text-gray-900 mb-2">No goals yet</h3>
                    <p className="text-gray-600 mb-6">Set your first financial goal to start tracking your progress.</p>
                    <button className="px-6 py-3 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors">
                        Create Your First Goal
                    </button>
                </div>
            )}
        </div>
    );
};

export default GoalsPage;
