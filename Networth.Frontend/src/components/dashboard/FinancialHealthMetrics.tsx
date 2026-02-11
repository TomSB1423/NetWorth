export function FinancialHealthMetrics() {
    return (
        <div className="relative min-h-[160px] flex items-center justify-center border border-dashed border-slate-700 rounded-lg p-4">
            <div className="text-center space-y-3 opacity-50">
                <h3 className="text-sm font-semibold text-white">
                    Financial Health
                </h3>
                <div className="grid grid-cols-2 gap-3">
                    <div className="h-14 bg-slate-700 rounded"></div>
                    <div className="h-14 bg-slate-700 rounded"></div>
                </div>
            </div>
            <div className="absolute inset-0 flex items-center justify-center bg-black/20 backdrop-blur-[1px] rounded-lg">
                <span className="bg-black/50 px-3 py-1.5 rounded text-white text-xs font-medium">
                    To be implemented
                </span>
            </div>
        </div>
    );
}
