import { BrowserRouter, Routes, Route } from 'react-router-dom';
import DashboardLayout from './layouts/DashboardLayout';
import Home from './pages/Home';
import Rooms from './pages/Rooms';
import Bookings from './pages/Bookings';
import Analytic from './pages/Analytic';
import Financial from './pages/Financial';
import Notifications from './pages/Notifications';
import  Reviews from './pages/Reviews';
import Promo from './pages/Promo';
import Staff from './pages/staff';
import RoomType from './pages/RoomType';
import { Toaster } from 'react-hot-toast';
import Modal from './ui/Modal';
import Login from './pages/Login';
import Register from './pages/Register';
function App() {
  return (
<BrowserRouter>
  <Modal>
    <Toaster
      position="top-right"
      containerStyle={{ zIndex: 9999999 }}
      toastOptions={{ duration: 4000 }}
    />

    <Routes>
      <Route index element={<Login />} />
      <Route path="login" element={<Login />} />
      <Route path="register" element={<Register />} />

      <Route element={<DashboardLayout />}>
        {/* <Route index element={<Home />} /> */}
        <Route path="dashboard" element={<Home />} />
        <Route path="rooms" element={<Rooms />} />
        <Route path="roomType" element={<RoomType />} />
        <Route path="bookings" element={<Bookings />} />
        <Route path="staff" element={<Staff />} />
        <Route path="financial" element={<Financial />} />
        <Route path="promo" element={<Promo />} />
        <Route path="analytic" element={<Analytic />} />
        <Route path="notification" element={<Notifications />} />
        <Route path="reviews" element={<Reviews />} />
      </Route>
    </Routes>
  </Modal>
</BrowserRouter>
  )
}

export default App
