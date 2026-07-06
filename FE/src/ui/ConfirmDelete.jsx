import React from 'react';

export default function ConfirmDelete({ resourceName = "this item", onConfirm, disabled }) {
  return (
    <div className="w-full max-w-md p-2 rounded-2xl flex flex-col gap-4">
      <h3 className="text-xl font-bold text-gray-800 capitalize">
        Delete {resourceName}
      </h3>
      
      <p className="text-gray-900 text-sm leading-relaxed">
        Are you sure you want to delete this {resourceName} permanently? This action cannot be undone.
      </p>
      
      <div className="flex justify-end gap-3 mt-2">

        
        <button
          type="button"
          disabled={disabled}
          onClick={onConfirm}
          className="px-4 py-2 text-sm font-medium bg-red-605 hover:bg-red-700  rounded-xl transition-all duration-200 shadow-sm disabled:bg-red-400 disabled:cursor-not-allowed"
        >
          {disabled ? "Deleting..." : "Delete"}
        </button>
      </div>
    </div>
  );
}