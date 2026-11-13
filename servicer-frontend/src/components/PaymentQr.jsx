import { QRCodeSVG } from "qrcode.react";
import { formatRupees, UPI_ID, upiPaymentLink } from "../utils/upi";

export default function PaymentQr({ appointment }) {
  const paymentLink = upiPaymentLink(appointment);
  if (!paymentLink) {
    return <p className="rounded-xl bg-gold-100 p-4 text-sm text-aub-900">UPI payment is not configured. Set VITE_UPI_ID and restart the frontend.</p>;
  }

  return (
    <div className="space-y-3 rounded-xl bg-ice-200 p-4 text-center text-sm text-aub-900">
      <p>Scan to pay <strong>{formatRupees(appointment.workerPrice)}</strong></p>
      <div className="mx-auto w-fit rounded-xl bg-white p-3">
        <QRCodeSVG value={paymentLink} size={208} level="M" includeMargin />
      </div>
      <p>Pay to <strong>{UPI_ID}</strong></p>
      <a className="btn btn-primary inline-flex" href={paymentLink}>Open UPI app</a>
      <p className="text-xs">Payment is sent through your UPI app. Servicer cannot verify the transfer automatically.</p>
    </div>
  );
}
