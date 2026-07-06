import Header from './Header'
import SideBar from './SideBar'
import { Outlet } from 'react-router-dom'

export default function DashboardLayout() {
  return (
    <div className="h-screen flex overflow-hidden">

      {/* Sidebar */}
      <aside className="hidden lg:block  shrink-0">
        <SideBar />
      </aside>

      {/* Right section */}
      <div className="flex flex-col flex-1 h-screen">

        {/* Fixed Header */}
        <header className=" shrink-0">
          <Header />
        </header>

        {/* Scrollable Main ONLY */}
        <main className="flex-1 overflow-y-auto px-2 bg-[var(--main)]">
          <Outlet />
        </main>

      </div>
    </div>
  )
}