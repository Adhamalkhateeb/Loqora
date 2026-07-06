import IconSideBar from "./IconSideBar";
import SideBarList from "./SideBarList";

export default function SideBar() {
  return (
    <aside className="w-[220px] h-screen border-r  flex flex-col bg-[var(--sidebar)] ">
      
      <div className=" h-15 flex items-center gap-2 px-4 border-b border-neutral-200 text-lg font-bold text-gray-800">
        <span className="bg-blue-700 rounded-xl px-1 text-white">SE</span>
        <span className="font-extrabold">StayEase</span>
      </div>
      
      
      <SideBarList/>
      
{/*       
      <div className=" border-t text-sm text-gray-600">
        Account
      </div> */}
    </aside>
  );
}