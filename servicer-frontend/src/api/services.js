import { api, tokenStore } from "./client";
import { ENDPOINTS } from "./endpoints";

export const authApi = {
  login: (email, password) => api(ENDPOINTS.login, { method: "POST", body: { email, password } }),
  registerUser: (form) => api(ENDPOINTS.registerUser, {
    method: "POST",
    body: {
      userName: form.userName.trim(), userEmail: form.email.trim(), mobileNo: form.phoneNumber,
      address: form.address.trim(), password: form.password, latitude: 0, longitude: 0,
    },
  }),
  registerWorker: (form) => api(ENDPOINTS.registerWorker, {
    method: "POST",
    body: {
      workerName: form.userName.trim(), workerEmail: form.email.trim(), mobileNo: form.phoneNumber,
      address: form.address.trim(), password: form.password, latitude: 0, longitude: 0,
      category: form.category, qualification: form.qualification.trim(), license: form.license.trim(),
      experience: Number(form.experienceYears), pricingModel: form.pricingModel,
      price: Number(form.price),
    },
  }),
};

export const appointmentApi = {
  book: (data) => api(ENDPOINTS.bookAppointment, { method: "POST", body: data }),
  accept: (id) => api(ENDPOINTS.accept(id), { method: "POST" }),
  reject: (id) => api(ENDPOINTS.reject(id), { method: "POST" }),
  start: (id, otp) => api(ENDPOINTS.startJob(id), { method: "POST", body: { otp } }),
  complete: (id) => api(ENDPOINTS.complete(id), { method: "POST" }),
  cannotComplete: (id, reason) => api(ENDPOINTS.cannotComplete(id), { method: "POST", body: { failureReason: reason } }),
  cancel: (id) => api(ENDPOINTS.cancel(id), { method: "POST" }),
  forUser: () => api(ENDPOINTS.userAppointments),
  forWorker: () => api(ENDPOINTS.workerAppointments),
  confirmPayment: (id) => api(ENDPOINTS.confirmPayment(id), { method: "POST" }),
};

export const workerApi = {
  search: ({ category = "", city = "", minRating = 0 } = {}) => {
    const query = new URLSearchParams();
    if (category) query.set("category", category);
    if (city) query.set("city", city);
    if (minRating) query.set("minRating", String(minRating));
    return api(`${ENDPOINTS.workers}?${query}`);
  },
  me: () => api(ENDPOINTS.workerMe),
  toggleAvailability: (_workerId, isAvailable) =>
    api(ENDPOINTS.availability, { method: "POST", body: { isAvailable } }),
};

export const paymentApi = {
  pay: (id, method) => api(ENDPOINTS.payment(id), { method: "POST", body: { method } }),
  get: (id) => api(ENDPOINTS.getPayment(id)),
};

export const ratingApi = {
  rate: (id, stars, comment) => api(ENDPOINTS.rating(id), { method: "POST", body: { stars, comment } }),
  get: (id) => api(ENDPOINTS.getRating(id)),
};

export const adminApi = {
  overview: () => api(ENDPOINTS.adminOverview),
  setBlocked: (workerId, blocked) => api(ENDPOINTS.blockWorker(workerId), { method: "POST", body: { value: blocked } }),
  setVerified: (workerId, verified) => api(ENDPOINTS.verifyWorker(workerId), { method: "POST", body: { value: verified } }),
};

export { tokenStore };
