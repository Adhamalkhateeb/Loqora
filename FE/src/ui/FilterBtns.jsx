import { useSearchParams } from "react-router-dom";

export default function FilterBtns({
  options = [],
  filterField
}) {

  const [searchParams, setSearchParams] =
    useSearchParams();

  const currentValue =
    searchParams.get(filterField) || "all";

  function handleChange(e) {

    searchParams.set(
      filterField,
      e.target.value
    );

    setSearchParams(searchParams);
  }

  return (
    <select
      value={currentValue}
      onChange={handleChange}
      className="
        w-40
        p-2
        rounded-xl
        bg-[var(--main)]
        border
        border-gray-200
        outline-none
        cursor-pointer
      "
    >
      {options.map((option) => (
        <option
          key={option.value}
          value={option.value}
        >
          {option.label}
        </option>
      ))}
    </select>
  );
}