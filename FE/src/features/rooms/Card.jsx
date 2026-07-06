import React from 'react'
import ContentCard from './ContentCard'

function Card({item , primaryImage,status}) {
  return (
          <div
            className="
            bg-white
            rounded-2xl
            overflow-hidden
            shadow-sm
            min-[500px]:w-full
            hover:shadow-xl
            transition-all
            duration-300
            cursor-pointer
          "
          >

            <div className="relative h-52">

              <img
                src={primaryImage || "/placeholder.jpg"}
                alt={item.roomType?.name}
                className="w-full h-full object-cover"
                 loading="lazy"
              />

            <span
              className={`
                absolute
                top-3
                right-3
                text-xs
                px-3
                py-1
                rounded-full
                text-white
                flex
                items-center
                gap-1
                ${status?.color}
              `}
            >
              {status?.icon}
              {status?.text}
            </span>

              <span
                className="
                absolute
                bottom-3
                left-3
                bg-black/70
                text-white
                px-3
                py-1
                rounded-lg
                text-sm
              "
              >
                ${item.roomType?.basePrice}/night
              </span>

            </div>

            <ContentCard item={item} />
</div>
  )
}
export default React.memo(Card);