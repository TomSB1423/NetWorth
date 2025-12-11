import {
    PieChart,
    Pie,
    Cell,
    ResponsiveContainer,
    Legend,
    Tooltip,
} from "recharts";
import { Loader2 } from "lucide-react";

const data = [
    { name: "Cash", value: 400 },
    { name: "Stocks", value: 300 },
    { name: "Real Estate", value: 300 },
    { name: "Crypto", value: 200 },
];

const COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042"];

interface AssetAllocationChartProps {
    isSyncing: boolean;
}

export function AssetAllocationChart({ isSyncing }: AssetAllocationChartProps) {
    if (isSyncing) {
        return (
            <div className="h-[300px] w-full flex items-center justify-center text-gray-400">
                <Loader2 className="w-8 h-8 animate-spin" />
                <span className="ml-2">Loading data...</span>
            </div>
        );
    }

    return (
        <div className="h-[300px] w-full relative">
            <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                    <Pie
                        data={data}
                        cx="50%"
                        cy="50%"
                        innerRadius={60}
                        outerRadius={80}
                        fill="#8884d8"
                        paddingAngle={5}
                        dataKey="value"
                    >
                        {data.map((_, index) => (
                            <Cell
                                key={`cell-${index}`}
                                fill={COLORS[index % COLORS.length]}
                            />
                        ))}
                    </Pie>
                    <Tooltip
                        contentStyle={{
                            backgroundColor: "#1F2937",
                            borderColor: "#374151",
                            color: "#F3F4F6",
                        }}
                        itemStyle={{ color: "#F3F4F6" }}
                    />
                    <Legend />
                </PieChart>
            </ResponsiveContainer>
            <div className="absolute inset-0 flex items-center justify-center bg-black/20 backdrop-blur-[1px] rounded-lg">
                <span className="bg-black/50 px-4 py-2 rounded text-white font-semibold">
                    To be implemented
                </span>
            </div>
        </div>
    );
}
