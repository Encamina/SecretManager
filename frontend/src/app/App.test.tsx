import { render, screen } from '@testing-library/react';
import { App } from './App';

describe('App', () => {
  it('renders the phase 1 bootstrap message', () => {
    render(<App />);

    expect(screen.getByRole('heading', { name: /secrets dashboard bootstrap/i })).toBeVisible();
  });
});
