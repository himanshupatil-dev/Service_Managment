export const UPI_ID = import.meta.env.VITE_UPI_ID || "";

export function upiPaymentLink(appointment) {
  if (!UPI_ID) return "";
  const params = new URLSearchParams({
    pa: UPI_ID,
    am: Number(appointment.workerPrice || 0).toFixed(2),
    cu: "INR",
    tn: `Servicer appointment ${appointment.appointmentId}`,
  });
  return `upi://pay?${params.toString()}`;
}

export function formatRupees(amount) {
  return Number(amount || 0).toLocaleString("en-IN", { style: "currency", currency: "INR" });
}
