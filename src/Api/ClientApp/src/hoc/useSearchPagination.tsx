import SearchBar from "@components/Ui/SearchBar";
import { Pagination, Select } from "@mantine/core";
import queryStringGenerator from "@utils/queryStringGenerator";
import React, { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";

export interface IWithSearchPagination {
  searchParams: string;
  pagination: (totalPage: number) => JSX.Element;
  searchComponent: (placeholder?: string) => JSX.Element;
  sortComponent: (
    data: {
      value: string;
      label: string;
    }[],
    placeholder: string
  ) => JSX.Element;
  filterComponent: (
    data: {
      value: string;
      label: string;
    }[],
    placeholder: string,
    key: string
  ) => JSX.Element;
  setInitialSearch: React.Dispatch<
    React.SetStateAction<
      {
        key: string;
        value: string;
      }[]
    >
  >;
}

const withSearchPagination =
  <P extends object>(Component: React.FC<P & IWithSearchPagination>) =>
  (props: P) => {
    const [params, setParams] = useSearchParams();
    const [initialSearch, setInitialSearch] = useState<
      {
        key: string;
        value: string;
      }[]
    >([
      {
        key: "",
        value: "",
      },
    ]);

    let search = params.get("s") ?? null;
    let pageSize = 12;
    const [filterValue, setFilterValue] = useState<string>("");
    const [currentPage, setCurrentPage] = useState(
      parseInt(params.get("p") ?? "1")
    );

    const qs = useMemo(() => {
      const qs = queryStringGenerator({
        search,
        page: currentPage,
        size: pageSize,
      });
      console.log(!search);
      !!search && params.set("s", search);

      pageSize && params.set("si", pageSize?.toString());
      currentPage && params.set("p", currentPage.toString());

      setParams(params, { replace: true });
      return qs;
    }, [currentPage, search, pageSize]);

    const setSearch = (search: string) => {
      for (let value of params.entries()) {
        if (value[0] !== "s") params.delete(value[0]);
      }
      params.set("s", search);
      setParams(params);
    };
    const sortComponent = (
      data: { value: string; label: string }[],
      placeholder: string
    ) => {
      return (
        <Select
          placeholder={placeholder}
          ml={5}
          clearable
          data={data}
          onChange={(e: string) => {
            // setSortValue(e);
          }}
        />
      );
    };

    const filterComponent = (
      data: { value: string; label: string }[],
      placeholder: string,
      key: string
    ) => {
      return (
        <Select
          placeholder={placeholder}
          ml={5}
          clearable
          maw={"184px"}
          value={filterValue}
          data={data}
          onChange={(e: string) => {
            setFilterValue(() => e);
            if (e) {
              // setFilterKey(() => key);
            } else {
              // setFilterKey(() => "");
            }
          }}
        />
      );
    };

    const setPage = (pageNumber: number) => {
      setCurrentPage(pageNumber);
    };

    const pagination = (totalPage: number) =>
      totalPage > 1 ? (
        <Pagination
          my={20}
          total={totalPage}
          page={currentPage}
          onChange={setPage}
        />
      ) : (
        <></>
      );
    const searchComponent = (placeholder?: string) => (
      <SearchBar
        search={search ?? ""}
        setSearch={setSearch}
        placeholder={placeholder}
      />
    );

    return (
      <Component
        {...(props as P)}
        filterComponent={filterComponent}
        searchParams={qs}
        pagination={pagination}
        searchComponent={searchComponent}
        sortComponent={sortComponent}
        setInitialSearch={setInitialSearch}
      />
    );
  };

export default withSearchPagination;
