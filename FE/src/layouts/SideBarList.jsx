import IconSideBar from "./IconSideBar";
import { IoBed } from "react-icons/io5";
import { HiOutlineViewGridAdd } from "react-icons/hi";
import { MdOutlineDateRange } from "react-icons/md";
import { FaUsers } from "react-icons/fa";
import { BsBellFill } from "react-icons/bs";
import { CiCreditCard1 } from "react-icons/ci";
import { IoPricetagOutline } from "react-icons/io5";
import { BsFillBarChartFill } from "react-icons/bs";
import { CiStar } from "react-icons/ci";
import { MdDifference } from "react-icons/md";

export default function SideBarList() {
  return (
      <div className="flex-1 overflow-y-auto py-6 px-4  ">
        <ul className="flex flex-col gap-5 ">
          <IconSideBar text='over view' link='/dashboard' icon={<HiOutlineViewGridAdd/>}/>
          <IconSideBar text='room inventory' link='/rooms' icon={<IoBed/>}/>
          <IconSideBar text='roomsTypes' link='/roomType' icon={<MdDifference/>}/>
          <IconSideBar text='bookings disk' link='/bookings' icon={<MdOutlineDateRange/>}/>
          <IconSideBar text='staff roster' link='/staff' icon={<FaUsers/>}/>
          <IconSideBar text='financial ledger' link='/financial' icon={<CiCreditCard1/>}/>
          <IconSideBar text='promo campaigns' link='/promo' icon={<IoPricetagOutline/>}/>
          <IconSideBar text='Hospitality Analytics' link='/analytic' icon={<BsFillBarChartFill/>}/>
          <IconSideBar text='notifications' link='/notification' icon={<BsBellFill/>}/>
          <IconSideBar text='reviews' link='/reviews' icon={<CiStar/>}/>
        </ul>
      </div>

  )
}
