import {
  cloneElement,
  createContext,
  useContext,
  useState,
} from "react";
import { IoIosCloseCircle } from "react-icons/io";

export const ModalContext = createContext();

export default function Modal({ children }) {
  const [openName, setOpenName] = useState("");

  const close = () => setOpenName("");
  const open = setOpenName;

  return (
    <ModalContext.Provider
      value={{
        openName,
        open,
        close,
      }}
    >
      {children}
    </ModalContext.Provider>
  );
}

/* ================= OPEN BUTTON ================= */

function Open({ children, opens }) {
  const { open } = useContext(ModalContext);

  return cloneElement(children, {
    onClick: () => open(opens),
  });
}

/* ================= MODAL WINDOW ================= */

function Window({ children, name }) {
  const { openName, close } = useContext(ModalContext);

  if (name !== openName) return null;

  return (
    <div
      className="
      fixed 
      inset-0 
      z-50 
      flex 
      items-center 
      justify-center 
      bg-black/30 
      backdrop-blur-sm
      px-4
      
    
    "
    >
      {/* Modal Box */}
      <div
        className="
        relative
        w-full
        max-w-lg
        rounded-2xl
        bg-white
        shadow-2xl
        p-5
        md:p-7
        animate-[fadeIn_.25s_ease]
      "
      >
        {/* Close Button */}
        <button
          onClick={close}
          className="
            absolute
            top-3
            right-3
            text-3xl
            text-gray-400
            hover:text-red-500
            transition
          "
        >
          <IoIosCloseCircle />
        </button>

        {/* Content */}
        <div className="mt-4">{children}</div>
      </div>
    </div>
  );
}


Modal.Open = Open;
Modal.Window = Window;