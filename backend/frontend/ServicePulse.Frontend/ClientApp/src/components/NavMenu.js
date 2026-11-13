import React, { useEffect, useState } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { Link } from 'react-router-dom';
import { authService } from '../services/authService';
import './NavMenu.css';

export function NavMenu() {
  const [collapsed, setCollapsed] = useState(true);
  const [user, setUser] = useState(authService.getUser());
  const [dropdownOpen, setDropdownOpen] = useState(false);

  useEffect(() => {
    const onStorage = (e) => {
      if (e.key === 'servicepulse_user') {
        setUser(e.newValue ? JSON.parse(e.newValue) : null);
      }
    };
    window.addEventListener('storage', onStorage);
    return () => window.removeEventListener('storage', onStorage);
  }, []);

  const toggleNavbar = () => setCollapsed((c) => !c);
  const toggleDropdown = () => setDropdownOpen((o) => !o);

  const handleLogout = () => {
    authService.clear();
    setUser(null);
    window.location.href = '/login';
  };

  return (
    <header>
      <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
        <NavbarBrand tag={Link} to="/dashboard">ServicePulse</NavbarBrand>
        <NavbarToggler onClick={toggleNavbar} className="mr-2" />
        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
          <ul className="navbar-nav flex-grow">
            {user && (
              <>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/dashboard">Dashboard</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/calls">Calls</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/calls/add">Add Call</NavLink>
                </NavItem>
              </>
            )}
          </ul>
          <ul className="navbar-nav ml-auto">
            <Dropdown isOpen={dropdownOpen} toggle={toggleDropdown}>
              <DropdownToggle nav caret style={{ cursor: 'pointer' }}>
                {user ? user.displayName : 'Account'}
              </DropdownToggle>
              <DropdownMenu>
                {user ? (
                  <>
                    <DropdownItem disabled>Signed in as {user.role}</DropdownItem>
                    <DropdownItem divider />
                    <DropdownItem onClick={handleLogout}>Logout</DropdownItem>
                  </>
                ) : (
                  <DropdownItem tag={Link} to="/login">Login</DropdownItem>
                )}
              </DropdownMenu>
            </Dropdown>
          </ul>
        </Collapse>
      </Navbar>
    </header>
  );
}
