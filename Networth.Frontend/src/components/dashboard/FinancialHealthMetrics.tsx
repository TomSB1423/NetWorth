export function FinancialHealthMetrics() {
    return (
        <div className="relative min-h-[200px] flex items-center justify-center border border-dashed border-slate-700 rounded-lg p-6">
            <div className="text-center space-y-4 opacity-50">
                <h3 className="text-lg font-semibold text-white">
                    Financial Health
                </h3>
                <div className="grid grid-cols-2 gap-4">
                    <div className="h-20 bg-slate-700 rounded"></div>
                    <div className="h-20 bg-slate-700 rounded"></div>
                </div>
            </div>
            <div className="absolute inset-0 flex items-center justify-center bg-black/20 backdrop-blur-[1px] rounded-lg">
                <span className="bg-black/50 px-4 py-2 rounded text-white font-semibold">
                    To be implemented
                </span>
            </div>
        </div>
    );
}
