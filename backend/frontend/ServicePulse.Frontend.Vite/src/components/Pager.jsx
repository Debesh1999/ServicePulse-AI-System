import React from 'react';
import { Pagination, PaginationItem, PaginationLink } from 'reactstrap';

export function Pager({ currentPage, totalPages, onPageChange, showPageInfo = true, pageSize = 10, totalCount }) {
  if (totalPages <= 1) {
    return null;
  }

  const pages = [];
  const maxVisible = 5;
  let startPage = Math.max(1, currentPage - Math.floor(maxVisible / 2));
  let endPage = Math.min(totalPages, startPage + maxVisible - 1);

  if (endPage - startPage + 1 < maxVisible) {
    startPage = Math.max(1, endPage - maxVisible + 1);
  }

  for (let i = startPage; i <= endPage; i++) {
    pages.push(i);
  }

  return (
    <div className="d-flex justify-content-between align-items-center mt-4">
      {showPageInfo && (
        <small className="text-muted">
          Page {currentPage} of {totalPages} ({totalCount || 0} records, {pageSize} per page)
        </small>
      )}
      <Pagination size="sm" className="mb-0">
        <PaginationItem disabled={currentPage === 1}>
          <PaginationLink
            onClick={() => onPageChange(currentPage - 1)}
            previous
          >
            Previous
          </PaginationLink>
        </PaginationItem>

        {startPage > 1 && (
          <PaginationItem>
            <PaginationLink onClick={() => onPageChange(1)}>1</PaginationLink>
          </PaginationItem>
        )}

        {startPage > 2 && (
          <PaginationItem disabled>
            <PaginationLink>...</PaginationLink>
          </PaginationItem>
        )}

        {pages.map((page) => (
          <PaginationItem key={page} active={page === currentPage}>
            <PaginationLink onClick={() => onPageChange(page)}>
              {page}
            </PaginationLink>
          </PaginationItem>
        ))}

        {endPage < totalPages - 1 && (
          <PaginationItem disabled>
            <PaginationLink>...</PaginationLink>
          </PaginationItem>
        )}

        {endPage < totalPages && (
          <PaginationItem>
            <PaginationLink onClick={() => onPageChange(totalPages)}>
              {totalPages}
            </PaginationLink>
          </PaginationItem>
        )}

        <PaginationItem disabled={currentPage === totalPages}>
          <PaginationLink
            onClick={() => onPageChange(currentPage + 1)}
            next
          >
            Next
          </PaginationLink>
        </PaginationItem>
      </Pagination>
    </div>
  );
}